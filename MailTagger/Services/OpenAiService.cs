using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MailTagger.Dtos;
using MailTagger.Helpers;
using MailTagger.Services.Interfaces;
using MailTagger.Settings;
using Microsoft.Extensions.Options;

namespace MailTagger.Services;

public class OpenAiService : ILLMService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _openAiSettings;

    public OpenAiService(IHttpClientFactory httpClientFactory, IOptions<OpenAiSettings> openAiSettings)
    {
        _httpClient = httpClientFactory.CreateClient();
        _openAiSettings = openAiSettings.Value;
        _httpClient.BaseAddress = new Uri(_openAiSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _openAiSettings.ApiKey);
    }

    public async Task<IEnumerable<MailTaggerResponseDto>> GenerateEmailTagsAsync(IEnumerable<MailTaggerRequestItemDto> list)
    {
        var requestData = new
        {
            model = _openAiSettings.Model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = OpenAiHelper.GetSystemPrompt(),
                },
                new
                {
                    role = "user",
                    content = GetUserMessage(list),
                }
            },
            tools = OpenAiHelper.GetToolPrompt(),
            tool_choice = "auto"
        };

        var jsonContent = JsonSerializer.Serialize(requestData);
        using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat/completions", content);
        var result = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var rootData = JsonDocument.Parse(result);
        var toolCalls = rootData.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("tool_calls").EnumerateArray();

        var responseList = new List<MailTaggerResponseDto>();

        foreach (var toolCall in toolCalls)
        {
            var arguments = toolCall.GetProperty("function").GetProperty("arguments").GetString()!;
            using var rootTags = JsonDocument.Parse(arguments);
            var tags = rootTags.RootElement.GetProperty("data");
            responseList.AddRange(JsonSerializer.Deserialize<List<MailTaggerResponseDto>>(tags.GetRawText())!);
        }
        return responseList;
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
}
