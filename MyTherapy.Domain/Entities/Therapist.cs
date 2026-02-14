using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class Therapist : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;
    public string LicenseDocumentPath { get; set; } = null!;

    public VerificationStatus VerificationStatus { get; set; }
    = VerificationStatus.Pending;

    public DateTime? VerifiedAt { get; set; }
}
