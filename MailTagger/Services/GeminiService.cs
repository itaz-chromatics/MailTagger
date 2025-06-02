using System.Text;
using System.Text.Json;
using MailTagger.Dtos;
using MailTagger.Helpers;
using MailTagger.Services.Interfaces;
using MailTagger.Settings;
using Microsoft.Extensions.Options;

namespace MailTagger.Services;

public class GeminiService : ILLMService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _geminiSettings;
    private readonly string _geminiEndpoint;

    public GeminiService(IHttpClientFactory httpClientFactory, IOptions<GeminiSettings> geminiSettings)
    {
        _geminiSettings = geminiSettings.Value;

        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(_geminiSettings.BaseUrl);
        _geminiEndpoint = $"v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
    }

    public async Task<IEnumerable<MailTaggerResponseDto>> GenerateEmailTagsAsync(IEnumerable<MailTaggerRequestItemDto> list)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SYSTEM PROMPT");
        sb.AppendLine(GeminiHelper.GetSystemPrompt());
        sb.AppendLine("USER MESSAGE");
        sb.AppendLine(GetUserMessage(list));
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new
                        {
                            text = sb.ToString()
                        }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.02,
                topK = 3,
                topP = 0.95,
                maxOutputTokens = 1024
            }
        };
        var jsonRequest = JsonSerializer.Serialize(requestBody);
        using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_geminiEndpoint, httpContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(responseBody);
        var text = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
        var cleanedText = ExtractJsonFromBackticks(text);
        using var parsedJson = JsonDocument.Parse(cleanedText);
        return JsonSerializer.Deserialize<List<MailTaggerResponseDto>>(parsedJson.RootElement.GetRawText())!;
    }

    private static string GetUserMessage(IEnumerable<MailTaggerRequestItemDto> list)
    {
        var sb = new StringBuilder();

        var i = 1;
        foreach (var item in list)
        {
            sb.AppendLine($"DATA {i}");
            sb.AppendLine($"EMAIL: {item.Email}");
            sb.AppendLine($"MESSAGE: {item.Message}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string ExtractJsonFromBackticks(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Remove triple backticks and optional "json" tag
        var lines = input.Split('\n')
                         .Where(line => !line.Trim().StartsWith("```"))
                         .ToList();

        return string.Join("\n", lines).Trim();
    }
}
