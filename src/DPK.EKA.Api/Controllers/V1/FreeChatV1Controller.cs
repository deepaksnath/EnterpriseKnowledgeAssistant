using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DPK.EKA.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/freechat")]
    [EnableRateLimiting("SlidingWindowPolicy")]
    public class FreeChatV1Controller : ControllerBase
    {
        private readonly IFreeChatService _freeChatService;
        private readonly ILogger<FreeChatV1Controller> _logger;

        public FreeChatV1Controller(IFreeChatService freeChatService, ILogger<FreeChatV1Controller> logger)
        {
            _freeChatService = freeChatService;
            _logger = logger;
        }

        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] RagRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Question is required.");
            }

            try
            {
                _logger.LogInformation("Chat query received: {Question}", request.Question);

                var response = await _freeChatService.GetAnswerAsync(request.Question);

                return Ok(new { Version = "v1", Data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat query");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
