using MyTherapy.Domain.Enums;

namespace MyTherapy.Application.DTOs.Therapists;

public class VerificationStatusResponse
{
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime? VerifiedAt { get; set; }
}