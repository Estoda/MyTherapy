using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using MyTherapy.Application.DTOs.Slots;
using System.Security.Claims;
using MyTherapy.Domain.Entities;
using MyTherapy.Domain.Enums;

namespace MyTherapy.API.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/patient/bookings")]
public class PatientBookingController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientBookingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(AppointmentResponse request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null ) 
            return Unauthorized("User ID not found in token."); 

        var patientId = Guid.Parse(userId); // Convert string to Guid
        var patient = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == Role.Patient);

        if (patient == null)
            return NotFound("Patient not found");

        var slot = await _context.AvailabilitySlots
            .Include(s => s.Therapist)
            .FirstOrDefaultAsync(s => s.Id == request.SlotId);

        if (slot == null)
            return NotFound("Slot not found");

        if (slot.IsBooked)
            return BadRequest("Slot already booked");


        var appointment = new Appointment
        {
            PatientId = patient.Id,
            TherapistId = slot.TherapistId,
            SlotId = slot.Id,
            AppointmentDateTime = slot.StartTime,
            DurationMinutes = (int)(slot.EndTime - slot.StartTime).TotalMinutes,
            Status = AppointmentStatus.Scheduled
        };

        slot.IsBooked = true;

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return Ok(appointment);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null) 
            return Unauthorized("User ID not found in token.");

        var patientId = Guid.Parse(userId); // Convert string to Guid
        var patient = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == Role.Patient);

        if (patient == null)
            return NotFound("Patient not found.");

        var appointments = await _context.Appointments
            .Include(a => a.Therapist)
            .ThenInclude(t => t.User)
            .Where(a => a.PatientId == patient.Id)
            .ToListAsync();

        var result = appointments.Select(a => new AppointmentResponse
        {
            SlotId = a.Id,
            TherapistName = a.Therapist.User.FullName,
            AppointmentDatetime = a.AppointmentDateTime,
            DurationMinutes = a.DurationMinutes,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        });

        return Ok(result);
    }
}
