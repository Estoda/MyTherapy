using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Application.DTOs.Users;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;

namespace MyTherapy.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context) => _context = context;

    private string? BuildPictureUrl(string? path) => path != null ? $"{Request.Scheme}://{Request.Host}/{path}" : null;

    [HttpGet("patients")]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _context.Patients
            .Include(p => p.User)
            .ToListAsync();

        var result = patients.Select(p => new UserSummaryResponse
        {
            Id = p.Id,
            FullName = p.User.FullName,
            ProfilePicture = BuildPictureUrl(p.User.ProfilePicture)
        });

        return Ok(result);
    }

    [HttpGet("therapists")]
    public async Task<IActionResult> GetAllTherapists()
    {
        var therapists = await _context.Therapists
            .Include(t => t.User)
            .Where(t => t.VerificationStatus == VerificationStatus.Approved)
            .ToListAsync();

        var result = therapists.Select(t => new UserSummaryResponse
        {
            Id = t.Id,
            FullName = t.User.FullName,
            ProfilePicture = BuildPictureUrl(t.User.ProfilePicture)
        });

        return Ok(result);
    }

    [HttpGet("patients/{id}")]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound($"Patient not found with id: {id}");

        return Ok(new PatientDetailsResponse
        {
            Id = patient.Id,
            FullName = patient.User.FullName,
            ProfilePicture = BuildPictureUrl(patient.User.ProfilePicture),
            Phone = patient.User.Phone,
            Gender = patient.User.Gender,
            DateOfBirth = patient.User.DateOfBirth,
            MentalHealthStatus = patient.MentalHealthStatus,
            TherapistPreferences = patient.TherapistPreferences
        });
    }

    [HttpGet("therapists/{id}")]
    public async Task<IActionResult> GetTherapistById(Guid id)
    {
        var therapist = await _context.Therapists
            .Include(t => t.User)
            .Where(t => t.VerificationStatus == VerificationStatus.Approved)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (therapist == null)
            return NotFound($"Therapist not found with id: {id}");

        return Ok(new TherapistDetailsResponse
        {
            Id = therapist.Id,
            FullName = therapist.User.FullName,
            ProfilePicture = BuildPictureUrl(therapist.User.ProfilePicture),
            LicenseNumber = therapist.LicenseNumber,
            LicenseDocumentPath = therapist.LicenseDocumentPath,
            Specialization = therapist.Specialization,
            ExperienceYears = therapist.ExperienceYears,
            PricePerSession = therapist.PricePerSession,
            RatingAverage = therapist.RatingAverage,
            TotalRatings = therapist.TotalRatings,
            TotalSessions = therapist.TotalSessions,
            Bio = therapist.Bio,
            AvailableDays = therapist.AvailableDays,
            Phone = therapist.User.Phone,
            Gender = therapist.User.Gender
        });
    }
}