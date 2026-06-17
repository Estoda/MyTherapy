using MyTherapy.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Users;

public class PatientDetailsResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Phone { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? MentalHealthStatus { get; set; }
    public string? TherapistPreferences { get; set; }
}
