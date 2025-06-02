using MailTagger.Dtos;

namespace MailTagger.Services.Interfaces;

public interface IMailTaggerService
{
    Task<IEnumerable<MailTaggerResponseDto>> GetMailTags(MailTaggerRequestDto dto);
}
