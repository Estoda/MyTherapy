using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MyTherapy.Application.DTOs.Payment;
using MyTherapy.Application.DTOs.Therapists;
using MyTherapy.Application.Interfaces;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using System.Security.Claims;

namespace MyTherapy.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IProfileService _profileService;

    public ProfileController(AppDbContext context, IProfileService profileService) => (_context, _profileService) = (context, profileService);

    [HttpGet("verification-status")]
    [Authorize(Roles = "Therapist")]
    public async Task<IActionResult> GetVerificationStatus()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var therapist = await _context.Therapists
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (therapist == null)
            return NotFound("Therapist not found.");

        return Ok(new VerificationStatusResponse
        {
            VerificationStatus = therapist.VerificationStatus,
            VerifiedAt = therapist.VerifiedAt
        });
    }

    [HttpPost("upload-picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var relativePath = await _profileService.UploadProfilePictureAsync(
            userId,
            file.OpenReadStream(),
            file.FileName,
            file.Length
        );

        return Ok(new
        {
            message = "Profile picture uploaded successfully.",
            profilePictureUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}"
        });
    }

    [HttpPost("upload-license")]
    [Authorize(Roles = "Therapist")]
    public async Task<IActionResult> UploadLicenseDocument(IFormFile file)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var relativePath = await _profileService.UploadLicenseDocumentAsync(
            userId,
            file.OpenReadStream(),
            file.FileName,
            file.Length
        );

        return Ok(new
        {
            message = "License document uploaded successfully.",
            licenseDocumentUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}"
        });
    }

    private const decimal PlatformCommissionRate = 0.10m;

    [HttpGet("earnings")]
    [Authorize(Roles = "Therapist")]
    public async Task<IActionResult> GetEarnings()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var therapist = await _context.Therapists
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (therapist == null)
            return NotFound("Therapist not found.");

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var successfulPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Successful)
            .Include(p => p.Appointment)
            .Where(p => p.Appointment.TherapistId == therapist.Id)
            .Include(p => p.User)
            .ToListAsync();

        decimal ApplyShare(decimal amount) => amount * (1 - PlatformCommissionRate);

        var todayEarnings = successfulPayments
            .Where(p => p.PaymentDate >= todayStart)
            .Sum(p => ApplyShare(p.Amount));

        var monthEarnings = successfulPayments
            .Where(p => p.PaymentDate >= monthStart)
            .Sum(p => ApplyShare(p.Amount));

        var totalEarnings = successfulPayments
            .Sum(p => ApplyShare(p.Amount));

        var recentPayments = successfulPayments
            .OrderByDescending(p => p.PaymentDate)
            .Take(10)
            .Select(p => new RecentPaymentResponse
            {
                PatientName = p.User.FullName,
                PatientProfilePicture = p.User.ProfilePicture != null
                ? $"{Request.Scheme}://{Request.Host}/{p.User.ProfilePicture}"
                : null,
                Amount = ApplyShare(p.Amount),
                PaymentDate = p.PaymentDate
            })
            .ToList();

        return Ok(new EarningsResponse
        {
            TodayEarnings = todayEarnings,
            ThisMonthEarnings = monthEarnings,
            TotalEarnings = totalEarnings,
            RecentPayments = recentPayments
        });
    }
}