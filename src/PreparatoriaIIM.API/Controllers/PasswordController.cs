using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.API.Models;
using PreparatoriaIIM.Application.Common.Interfaces;
using PreparatoriaIIM.Domain.Entities;

namespace PreparatoriaIIM.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : BaseApiController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PasswordController> _logger;

        public PasswordController(
            UserManager<Usuario> userManager,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<PasswordController> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // No revelar que el usuario no existe o no está confirmado
                    return Ok(new { Message = "Si tu correo está registrado, te enviaremos un enlace para restablecer tu contraseña" });
                }

                // Generar token de restablecimiento de contraseña
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                
                // Crear URL de restablecimiento
                var baseUrl = _configuration["Frontend:BaseUrl"] ?? "https://tudominio.com";
                var callbackUrl = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

                // Enviar correo electrónico
                await _emailService.SendPasswordResetEmailAsync(user.Email, token, callbackUrl);

                return Ok(new { Message = "Se ha enviado un enlace de restablecimiento de contraseña a tu correo electrónico" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la solicitud de restablecimiento de contraseña");
                return StatusCode(500, "Error interno del servidor al procesar la solicitud");
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // No revelar que el usuario no existe
                    return Ok(new { Message = "Si tu correo está registrado, tu contraseña ha sido restablecida" });
                }

                // Decodificar el token
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
                
                // Restablecer la contraseña
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
                
                if (!result.Succeeded)
                {
                    return BadRequest(new { Errors = result.Errors });
                }

                return Ok(new { Message = "Tu contraseña ha sido restablecida exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restablecer la contraseña");
                return StatusCode(500, "Error interno del servidor al restablecer la contraseña");
            }
        }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
