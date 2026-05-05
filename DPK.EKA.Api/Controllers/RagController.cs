using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPK.EKA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RagController : ControllerBase
    {
        private readonly IRagService _ragService;
        private readonly ILogger<RagController> _logger;

        public RagController(IRagService ragService, ILogger<RagController> logger)
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

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RAG query");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
