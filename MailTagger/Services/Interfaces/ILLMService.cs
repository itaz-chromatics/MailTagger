using MailTagger.Dtos;

namespace MailTagger.Services.Interfaces;

public interface ILLMService
{
    Task<IEnumerable<MailTaggerResponseDto>> GenerateEmailTagsAsync(IEnumerable<MailTaggerRequestItemDto> list);
}
