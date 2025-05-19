using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.Application.Common.Interfaces;

namespace PreparatoriaIIM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : BaseApiController
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailService emailService,
            ILogger<EmailController> logger)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (request == null)
            {
                return BadRequest("La solicitud no puede ser nula");
            }

            try
            {
                await _emailService.SendEmailAsync(
                    request.To,
                    request.Subject,
                    request.Body,
                    request.IsHtml);

                _logger.LogInformation("Correo electrónico enviado exitosamente a {To}", request.To);
                return Ok(new { message = "Correo electrónico enviado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo electrónico a {To}", request.To);
                return StatusCode(500, new { message = "Error al enviar el correo electrónico", error = ex.Message });
            }
        }
    }

    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
    }
}
