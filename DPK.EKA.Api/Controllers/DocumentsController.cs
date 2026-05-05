using DPK.EKA.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DPK.EKA.Api.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentIngestionService _service;

        public DocumentsController(IDocumentIngestionService service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");
            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are supported");
            const long maxSize = 3 * 1024 * 1024; // 10 MB
            if (file.Length > maxSize)
                return BadRequest("File too large");

            using var stream = file.OpenReadStream();
            var result = await _service.ProcessAsync(stream, file.FileName);

            return Ok(result);
        }
    }
}
