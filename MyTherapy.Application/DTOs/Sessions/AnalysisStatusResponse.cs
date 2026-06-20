namespace MyTherapy.Application.DTOs.Sessions;

public class AnalysisStatusResponse
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AiEmotionSummary { get; set; }
}
