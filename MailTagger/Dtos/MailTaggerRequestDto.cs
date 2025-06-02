using MailTagger.Enums;

namespace MailTagger.Dtos;

public class MailTaggerRequestDto
{
    public IEnumerable<MailTaggerRequestItemDto> Data { get; set; }
    public LLMProvider LLMProvider { get; set; }
}

public class MailTaggerRequestItemDto
{
    public string Email { get; set; }
    public string Message { get; set; }
}
