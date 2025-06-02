using MailTagger.Dtos;
using MailTagger.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailTagger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailTaggerController(IMailTaggerService mailTaggerService) : ControllerBase
    {
        private readonly IMailTaggerService _mailTaggerService = mailTaggerService;

        /// <summary>
        /// Get Mail Tags. POST request as request body is a unknown length list.
        /// </summary>
        /// <returns>List with email and their tags.</returns>
        [HttpPost(Name = "GetMailTags")]
        public async Task<ActionResult<IEnumerable<MailTaggerResponseDto>>> GetMailTags([FromBody] MailTaggerRequestDto dto)
        {
            var response = await _mailTaggerService.GetMailTags(dto);
            return Ok(response);
        }
    }
}
