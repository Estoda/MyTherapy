namespace MyTherapy.Application.DTOs.Slots;

public class SlotResponse
{
    public Guid SlotId { get; set; }
    public string? TherapistName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }

}
