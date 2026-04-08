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

[Authorize(Roles = "Therapist")]
[ApiController]
[Route("api/therapist/availability")]
public class TherapistAvailabilityController : Controller
{
    private readonly AppDbContext _context;
    
    public TherapistAvailabilityController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> CreateSlot(CreateSlotRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the therapist's user ID from the claims

        var therapist = await _context.Therapists.FirstOrDefaultAsync(t => t.UserId.ToString() == userId);

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

        return Ok(slot);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMySlots()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var therapist = await _context.Therapists.FirstOrDefaultAsync(t => t.UserId.ToString() == userId);

        if (therapist == null)
            return NotFound("Therapist not found.");

        var slots = await _context.AvailabilitySlots.Where(s => s.TherapistId == therapist.Id).ToListAsync();

        return Ok(slots);
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
