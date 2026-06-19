namespace MyTherapy.Application.Interfaces;

public interface IAiAnalysisService
{
    Task<string> SubmitRecordingAsync(Stream fileStream, string fileName);
    Task<AiTaskResult> CheckTaskStatusAsync(string TaskId);
}


public class AiTaskResult 
{
    public string Status { get; set; } = string.Empty;
    public string? ResultJson { get; set; }
    public string? ErrorDetail { get; set; }
}