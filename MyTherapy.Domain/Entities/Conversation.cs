using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class Conversation : BaseEntity
{
    public Guid PatientId { get; set; }
    public PatientProfile Patient { get; set; } = null!;

    public Guid TherapistId { get; set; }
    public TherapistProfile Therapist { get; set; } = null!;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }

    // Navigation
    public ICollection<Message> Messages { get; set; } = [];
}
