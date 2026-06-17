using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using MyTherapy.Application.DTOs.Slots;
using System.Security.Claims;
using MyTherapy.Domain.Entities;

namespace MyTherapy.API.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/patient/availability")]
public class PatientAvailabilityController : Controller
{
    private readonly AppDbContext _context;
    
    public PatientAvailabilityController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots()
    {

        var now = DateTime.UtcNow;

        var slots = await _context.AvailabilitySlots
        .Where(s => !s.IsBooked && s.StartTime > now)
        .Include(s => s.Therapist)
        .ThenInclude(t => t.User)
        .ToListAsync();

        var result = slots.Select(s => new PatientSlotResponse
        {
            SlotId = s.Id,
            TherapistId = s.TherapistId,
            TherapistName = s.Therapist.User.FullName,
            TherapistProfilePicture = s.Therapist.User.ProfilePicture != null ? $"{Request.Scheme}://{Request.Host}/{s.Therapist.User.ProfilePicture}" : null,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            IsBooked = s.IsBooked,
        }).ToList();

        return Ok(result);
    }
}
