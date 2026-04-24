using MyTherapy.Domain.Common;
using System.Text.Json.Serialization;

namespace MyTherapy.Domain.Entities;

public class AvailabilitySlot : BaseEntity 
{
    public Guid TherapistId {get; set;}

    [JsonIgnore] // Breaks Cycle Error
    public TherapistProfile Therapist {get; set;} = null!;

    public DateTime StartTime {get; set;}
    public DateTime EndTime {get; set;}

    public bool IsBooked {get; set;} = false;
}
