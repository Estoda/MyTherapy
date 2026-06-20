using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Application.DTOs.Sessions;
using MyTherapy.Application.Interfaces;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using System.Security.Claims;

namespace MyTherapy.API.Controllers;

[Authorize]
[ApiController]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAiAnalysisService _aiService;
    private readonly IWebHostEnvironment _env;

    public SessionController(AppDbContext context, IAiAnalysisService aiService, IWebHostEnvironment env)
    {
        _context = context;
        _aiService = aiService;
        _env = env;
    }

    [Authorize(Roles = "Therapist")]
    [HttpPost("{sessionId}/upload-recording")]
    public async Task<IActionResult> UploadRecording(Guid sessionId, IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var session = await _context.Sessions
            .Include(s => s.Appointment)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return NotFound("Session not found.");

        var therapist = await _context.Therapists.FirstOrDefaultAsync(t => t.UserId == userId);
        if (therapist == null || session.Appointment.TherapistId != therapist.Id)
            return Unauthorized("You are not the therapist for this session.");

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".wav")
            return BadRequest("Only .wav files are allowed.");

        // Save file locally
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "recordings");
        Directory.CreateDirectory(uploadsFolder);

        var savedFileName = $"{sessionId}{extension}";
        var fullPath = Path.Combine(uploadsFolder, savedFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        session.RecordingLink = $"uploads/recordings/{savedFileName}";

        // Submit to AI service (fire-and-forget from the caller's perspective)
        using var aiStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        string taskId;
        try
        {
            taskId = await _aiService.SubmitRecordingAsync(aiStream, savedFileName);
        }
        catch (Exception ex)
        {
            session.AnalysisStatus = SessionAnalysisStatus.Failed;
            await _context.SaveChangesAsync();
            return StatusCode(500, new { message = $"AI service error: {ex.Message}" });
        }

        session.AiTaskId = taskId;
        session.AnalysisStatus = SessionAnalysisStatus.Pending;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Recording uploaded and analysis started.", sessionId = session.Id });
    }

    [HttpGet("{sessionId}/analysis-status")]
    public async Task<IActionResult> GetAnalysisStatus(Guid sessionId)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return NotFound("Session not found.");

        // If already Done or Failed, just return stored data — no need to call AI again
        if (session.AnalysisStatus == SessionAnalysisStatus.Done ||
            session.AnalysisStatus == SessionAnalysisStatus.Failed)
        {
            return Ok(new AnalysisStatusResponse
            {
                SessionId = session.Id,
                Status = session.AnalysisStatus.ToString(),
                AiEmotionSummary = session.AiEmotionSummary
            });
        }

        if (session.AnalysisStatus == SessionAnalysisStatus.Pending || session.AiTaskId == null)
        {
            return Ok(new AnalysisStatusResponse
            {
                SessionId = session.Id,
                Status = session.AnalysisStatus.ToString()
            });
        }

        AiTaskResult result;
        try
        {
            result = await _aiService.CheckTaskStatusAsync(session.AiTaskId);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                message = "AI analysis service is currently unreachable. Please try again later.",
                detail = ex.Message
            });
        }

        // Status is Processing — check with AI

        if (result.Status == "completed")
        {
            session.AiEmotionSummary = result.ResultJson;
            session.AnalysisStatus = SessionAnalysisStatus.Done;
            await _context.SaveChangesAsync();
        }
        else if (result.Status == "failed")
        {
            session.AnalysisStatus = SessionAnalysisStatus.Failed;
            await _context.SaveChangesAsync();
        }
        // else still processing — leave as is

        return Ok(new AnalysisStatusResponse
        {
            SessionId = session.Id,
            Status = session.AnalysisStatus.ToString(),
            AiEmotionSummary = session.AnalysisStatus == SessionAnalysisStatus.Done ? session.AiEmotionSummary : null
        });
    }

    [HttpGet("by-appointment/{appointmentId}")]
    public async Task<IActionResult> GetSessionsByAppointment(Guid appointmentId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointment = await _context.Appointments
            .Include(a => a.Session)
            .Include(a => a.Patient)
            .Include(a => a.Therapist)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null)
            return NotFound("Appointment not found.");

        var isPatient = appointment.Patient.UserId == userId;
        var isTherapist = appointment.Therapist.UserId == userId;

        if (!isPatient && !isTherapist)
            return Forbid();

        if (appointment.Session == null)
            return NotFound("Session not found for this appointment.");

        return Ok(new SessionResponse
        {
            SessionId = appointment.Session.Id,
            AppointmentId = appointment.Id,
            AnalysisStatus = appointment.Session.AnalysisStatus,
            RecordingLink = appointment.Session.RecordingLink != null
            ? $"{Request.Scheme}://{Request.Host}/{appointment.Session.RecordingLink}"
            : null
        });
    }
}