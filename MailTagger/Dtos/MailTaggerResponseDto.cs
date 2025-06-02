using System.Text.Json.Serialization;

namespace MailTagger.Dtos;

public class MailTaggerResponseDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }
}
