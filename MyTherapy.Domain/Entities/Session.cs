using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class Session : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public string? SessionLink {get; set;}
    public string? RecordingLink {get; set;}
    public string? AiEmotionSummary {get; set;} // JSON
    public SessionAnalysisStatus AnalysisStatus { get; set; } = SessionAnalysisStatus.Pending;

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public string? TherapistFeedback { get; set; }
    public string? PatientFeedback { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
}
