using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTherapy.Application.Interfaces;
using System.Security.Claims;

namespace MyTherapy.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
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
}