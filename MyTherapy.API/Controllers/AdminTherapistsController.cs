using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;


namespace MyTherapy.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/therapists")]
public class AdminTherapistsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminTherapistsController(AppDbContext context) => _context = context;

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending() // Get therapists with pending verification
    {
        var therapists = await _context.Therapists
            .Include(t => t.User)
            .Where(t => t.VerificationStatus == VerificationStatus.Pending)
            .ToListAsync();

        return Ok(therapists);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var therapist = await _context.Therapists.FindAsync(id);
        if (therapist == null) return NotFound();

        therapist.VerificationStatus = VerificationStatus.Approved;
        therapist.VerifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var therapist = await _context.Therapists.FindAsync(id);
        if (therapist == null) return NotFound();

        therapist.VerificationStatus = VerificationStatus.Rejected;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
