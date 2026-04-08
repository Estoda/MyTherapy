using MyTherapy.Domain.Common;

namespace MyTherapy.Domain.Entities;

public class AvailabilitySlot : BaseEntity 
{
    public Guid TherapistId {get; set;}
    public Therapist Therapist {get; set;} = null!;

    public DateTime StartTime {get; set;}
    public DateTime EndTime {get; set;}

    public bool IsBooked {get; set;} = false;
}
