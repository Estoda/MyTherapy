using MyTherapy.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Users;

public class TherapistDetailsResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Phone { get; set; }
    public Gender? Gender { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string? LicenseDocumentPath { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal PricePerSession { get; set; }
    public decimal RatingAverage { get; set; }
    public int TotalRatings { get; set; }
    public int TotalSessions { get; set; }
    public string? Bio { get; set; }
    public string? AvailableDays { get; set; }
}
