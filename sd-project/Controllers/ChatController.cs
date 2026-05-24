using EventBooking.Models.ViewModels;
using EventBooking.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            var response = await _chatbotService.GetResponseAsync(request.Message);

            return Ok(new { response = response });
        }
    }
}
