using MailTagger.Dtos;
using MailTagger.Enums;
using MailTagger.Services.Interfaces;

namespace MailTagger.Services;

public class MailTaggerService(IServiceScopeFactory serviceScopeFactory) : IMailTaggerService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;


    public async Task<IEnumerable<MailTaggerResponseDto>> GetMailTags(MailTaggerRequestDto dto)
    {
        var lLMService = GetLLMService(dto.LLMProvider);
        return await lLMService.GenerateEmailTagsAsync(dto.Data);
    }

    private ILLMService GetLLMService(LLMProvider lLMProvider)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        return lLMProvider switch
        {
            LLMProvider.OpenAi => serviceProvider.GetRequiredService<OpenAiService>(),
            LLMProvider.Gemini => serviceProvider.GetRequiredService<GeminiService>(),
            _ => throw new NotImplementedException("Provider not implemented."),
        };
    }
}
