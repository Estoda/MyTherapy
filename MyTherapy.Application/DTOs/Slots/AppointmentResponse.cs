namespace MyTherapy.Application.DTOs.Slots;

public class AppointmentResponse
{
    public Guid SlotId { get; set; }
    public string? TherapistName { get; set; } = null!;
    public DateTime AppointmentDatetime { get; set; }
    public int? DurationMinutes { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
}
