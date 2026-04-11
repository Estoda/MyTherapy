using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class TherapistProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Specialization { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public string LicenseDocumentPath { get; set; } = null!;
    public int ExperienceYears { get; set; }
    public decimal PricePerSession { get; set; }
    public decimal RatingAverage { get; set; } = 0;
    public int TotalRatings { get; set; } = 0;
    public int TotalSessions { get; set; } = 0;
    public string? Bio { get; set; }
    public bool Available { get; set; } = true;
    public string? AvailableDays { get; set; } // json string representing available days and time slots, e.g. {"Monday": ["9:00-12:00", "14:00-18:00"], "Wednesday": ["10:00-16:00"]}

    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public DateTime? VerifiedAt { get; set; }

    // Navigation properties
    public ICollection<AvailabilitySlot> AvailabilitySlots {get; set;} = [];
}
