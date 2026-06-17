namespace MyTherapy.Application.DTOs.Slots;

public class TherapistSlotResponse
{
    public Guid SlotId { get; set; }
    public Guid TherapistId { get; set; }
    public string TherapistName { get; set; } = string.Empty;
    public string? TherapistProfilePicture { get; set; }
    public string? PatientName { get; set; }
    public string? PatientProfilePicture { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }

}
