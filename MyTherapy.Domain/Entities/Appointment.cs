using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public PatientProfile Patient { get; set; } = null!; 
    
    public Guid TherapistId { get; set; }
    public TherapistProfile Therapist { get; set; } = null!;

    public DateTime AppointmentDateTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public Guid? PaymentId { get; set; }
    public Payment? Payment { get; set; }

    public string? Notes { get; set; }
    public int? DurationMinutes { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Session? Session { get; set; }

    public Guid SlotId { get; set; }
    public AvailabilitySlot Slot { get; set; } = null!;
}
