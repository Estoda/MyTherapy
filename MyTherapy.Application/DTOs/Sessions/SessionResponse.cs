using MyTherapy.Domain.Enums;

namespace MyTherapy.Application.DTOs.Sessions;

public class SessionResponse
{
    public Guid SessionId { get; set; }
    public Guid AppointmentId { get; set; }
    public SessionAnalysisStatus AnalysisStatus { get; set; }
    public string? RecordingLink { get; set; }
}
