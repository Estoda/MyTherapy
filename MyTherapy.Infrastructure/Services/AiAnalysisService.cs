using MyTherapy.Application.Interfaces;
using System.Text.Json;

namespace MyTherapy.Infrastructure.Services;

public class AiAnalysisService : IAiAnalysisService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://am4magdy-gp.hf.space";

    public AiAnalysisService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<string> SubmitRecordingAsync(Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
        content.Add(fileContent, "files", fileName);

        var response = await _httpClient.PostAsync($"{BaseUrl}/analyze_session_batch/", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.GetProperty("success").GetBoolean())
            throw new Exception("AI Service rejected the file.");

        return root.GetProperty("task_id").GetString()!;
    }

    public async Task<AiTaskResult> CheckTaskStatusAsync(string taskId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/check_task/{taskId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var status = root.GetProperty("status").GetString();

        if (status == "complated")
        {
            return new AiTaskResult
            {
                Status = "complated",
                ResultJson = json
            };
        }

        if (status == "failed")
        {
            return new AiTaskResult
            {
                Status = "failed",
                ErrorDetail = root.TryGetProperty("detail", out var detail) ? detail.GetString() : "Unknown error"
            }; 
        }

        return new AiTaskResult { Status = "Processing" };
    }
}
