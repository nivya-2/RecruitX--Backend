using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IEmailTemplateService _service;

        public EmailTemplatesController(IEmailTemplateService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmailTemplateDto>>> GetAllTemplates()
        {
            var templates = await _service.GetAllTemplatesAsync();
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmailTemplateDto>> GetTemplateById(int id)
        {
            var template = await _service.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

    }
}
