using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class PatientProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string? MentalHealthStatus { get; set; }
    public string? Goals { get; set; }

    public Guid? PreferredTherapistId { get; set; }
    public TherapistProfile? PreferredTherapist { get; set; }
    public string? TherapistPreferences {get; set;}
}
