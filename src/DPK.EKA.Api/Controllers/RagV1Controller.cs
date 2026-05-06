using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DPK.EKA.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/rag")]
    [EnableRateLimiting("SlidingWindowPolicy")]
    public class RagV1Controller : ControllerBase
    {
        private readonly IRagService _ragService;
        private readonly ILogger<RagV1Controller> _logger;

        public RagV1Controller(IRagService ragService, ILogger<RagV1Controller> logger)
        {
            _ragService = ragService;
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
                _logger.LogInformation("RAG query received: {Question}", request.Question);

                var response = await _ragService.GetAnswerAsync(request.Question);

                return Ok(new { Version = "v1", Data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RAG query");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
