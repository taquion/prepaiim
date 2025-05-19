using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;
using PreparatoriaIIM.API.Models;
using PreparatoriaIIM.Domain.Entities;

namespace PreparatoriaIIM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope("access_as_user")]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<Usuario> userManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user == null)
                    return NotFound("Usuario no encontrado");

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    ApellidoPaterno = user.ApellidoPaterno,
                    ApellidoMaterno = user.ApellidoMaterno,
                    FechaNacimiento = user.FechaNacimiento,
                    Roles = roles,
                    Activo = user.Activo,
                    FechaCreacion = user.FechaCreacion,
                    FechaActualizacion = user.FechaActualizacion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del usuario actual");
                return StatusCode(500, "Error interno del servidor al obtener la información del usuario");
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound("Usuario no encontrado");

                // Actualizar solo los campos permitidos
                user.Nombre = model.Nombre ?? user.Nombre;
                user.ApellidoPaterno = model.ApellidoPaterno ?? user.ApellidoPaterno;
                user.ApellidoMaterno = model.ApellidoMaterno ?? user.ApellidoMaterno;
                user.FechaNacimiento = model.FechaNacimiento ?? user.FechaNacimiento;
                user.FechaActualizacion = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { Message = "Perfil actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el perfil del usuario");
                return StatusCode(500, "Error interno del servidor al actualizar el perfil");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound("Usuario no encontrado");

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { Message = "Contraseña actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar la contraseña");
                return StatusCode(500, "Error interno del servidor al cambiar la contraseña");
            }
        }
    }

    public class UpdateProfileModel
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }

    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
