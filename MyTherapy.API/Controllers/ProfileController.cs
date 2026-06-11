using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Application.DTOs.Therapists;
using MyTherapy.Application.Interfaces;
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

}