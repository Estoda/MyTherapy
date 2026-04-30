using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Application.DTOs.Reviews;
using MyTherapy.Domain.Entities;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using System.Security.Claims;

namespace MyTherapy.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewController(AppDbContext context) => _context = context;

    [Authorize(Roles = "Patient")]
    [HttpPost]
    public async Task<IActionResult> CreateReview(CreateReviewRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
        var appointment = await _context.Appointments.FindAsync(request.AppointmentId);

        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest("Rating must be between 1 and 5.");

        if (patient == null)
            return NotFound("Patient profile not found.");

        if (appointment == null)
            return NotFound("Appointment not found.");


        if (appointment.PatientId != patient.Id)
            return BadRequest("You can only review appointments you attended.");

        if (appointment.Status != AppointmentStatus.Completed)
            return BadRequest("You can only review completed appointments.");

        var alreadyReviewed = await _context.Reviews.AnyAsync(r => r.AppointmentId == appointment.Id && r.PatientId == patient.Id);
        var therapist = await _context.Therapists.FindAsync(appointment.TherapistId);

        if (alreadyReviewed)
            return BadRequest("You have already reviewed this appointment.");

        if (therapist == null)
            return NotFound("Therapist not found.");

        var review = new Review
        {
            PatientId = patient.Id,
            TherapistId = appointment.TherapistId,
            AppointmentId = appointment.Id,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _context.Reviews.Add(review);


        therapist.TotalRatings += 1;
        therapist.RatingAverage = (decimal)(
            ((double)therapist.RatingAverage * (therapist.TotalRatings - 1) + request.Rating)
            / therapist.TotalRatings);

        therapist.RatingAverage = ((therapist.RatingAverage * (therapist.TotalRatings - 1) + request.Rating) / therapist.TotalRatings);

        await _context.SaveChangesAsync();

        return Ok(new ReviewResponse
        {
            Id = review.Id,
            PatientName = patient.User?.FullName ?? "Anonymous",
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        });
    }

    [HttpGet("therapist/{therapistId}")]
    public async Task<IActionResult> GetTherapistReviews(Guid TherapistId)
    {
        var therapist = await _context.Therapists
            .FirstOrDefaultAsync(t => t.Id == TherapistId);

        if (therapist == null)
            return NotFound("Therapist not found.");

        var reviews = await _context.Reviews
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Where(r => r.TherapistId == TherapistId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var result = new
        {
            TherapistId = therapist.Id,
            RatingAverage = therapist.RatingAverage,
            TotalRatings = therapist.TotalRatings,
            Reviews = reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                PatientName = r.Patient?.User?.FullName ?? "Anonymous",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
        };

        return Ok(result);
    }
};