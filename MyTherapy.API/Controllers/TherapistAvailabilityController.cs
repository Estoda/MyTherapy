using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MyTherapy.Infrastructure.Persistence;
using MyTherapy.Application.DTOs.Slots;
using System.Security.Claims;
using MyTherapy.Domain.Entities;


namespace MyTherapy.API.Controllers;

[Authorize(Roles = "Therapist")]
[ApiController]
[Route("api/therapist/availability")]
public class TherapistAvailabilityController : Controller
{
    private readonly AppDbContext _context;
    
    public TherapistAvailabilityController(AppDbContext context) => _context = context;
    [HttpGet("my")]
    public async Task<IActionResult> GetMySlots()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var now = DateTime.UtcNow;
        var therapist = await _context.Therapists.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId);

        if (therapist == null)
            return NotFound("Therapist not found.");

        var slots = await _context.AvailabilitySlots
            .Where(s => s.TherapistId == therapist.Id && s.StartTime > now)
            .Include(s => s.Appointments)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(p => p.User)
            .ToListAsync();

        // Map to DTOs 
        var result = slots.Select(s =>
        {
            var appointment = s.Appointments.FirstOrDefault();
            return new TherapistSlotResponse
            {
                SlotId = s.Id,
                TherapistId = therapist.Id,
                TherapistName = therapist.User.FullName,
                TherapistProfilePicture = therapist.User.ProfilePicture != null
                    ? $"{Request.Scheme}://{Request.Host}/{therapist.User.ProfilePicture}"
                    : null,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsBooked = s.IsBooked,
                PatientName = appointment?.Patient.User.FullName,
                PatientProfilePicture = appointment?.Patient.User.ProfilePicture != null
                    ? $"{Request.Scheme}://{Request.Host}/{appointment.Patient.User.ProfilePicture}"
                    : null
            };
        }).ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSlot(CreateSlotRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the therapist's user ID from the claims
        var now = DateTime.UtcNow;
        var therapist = await _context.Therapists.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId.ToString() == userId);

        if (request.StartTime <= now)
            return BadRequest("Start time must be in the future.");

        if (request.StartTime >= request.EndTime)
            return BadRequest("End time must be after start time.");

        if (therapist == null)
            return NotFound("Therapist not found.");

        if (request.StartTime >= request.EndTime)
            return BadRequest("Start time must be before end time.");

        var slot = new AvailabilitySlot
        {
            TherapistId = therapist.Id,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        _context.AvailabilitySlots.Add(slot);
        await _context.SaveChangesAsync();

        return Ok(new TherapistSlotResponse
        {
            SlotId = slot.Id,
            TherapistId = slot.TherapistId,
            TherapistName = slot.Therapist.User.FullName,
            TherapistProfilePicture = therapist.User.ProfilePicture != null
        ? $"{Request.Scheme}://{Request.Host}/{therapist.User.ProfilePicture}"
        : null,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            IsBooked = slot.IsBooked
        });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSlot(Guid id)
    {
        var slot = await _context.AvailabilitySlots.FindAsync(id);

        if (slot == null)
            return NotFound("Slot not found.");

        _context.AvailabilitySlots.Remove(slot);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
