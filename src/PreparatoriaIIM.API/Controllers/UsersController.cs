using System;
using System.Collections.Generic;
using System.Linq;
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
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope("access_as_user")]
    public class UsersController : BaseApiController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<Usuario> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _userManager.Users.OrderBy(u => u.ApellidoPaterno).ThenBy(u => u.Nombre);
                
                var totalItems = await query.CountAsync();
                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Nombre = u.Nombre,
                        ApellidoPaterno = u.ApellidoPaterno,
                        ApellidoMaterno = u.ApellidoMaterno,
                        Activo = u.Activo,
                        FechaCreacion = u.FechaCreacion
                    })
                    .ToListAsync();

                return HandlePagedResult(users, pageNumber, pageSize, totalItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios");
                return StatusCode(500, "Error interno del servidor al obtener los usuarios");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => u.Id == id)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Nombre = u.Nombre,
                        ApellidoPaterno = u.ApellidoPaterno,
                        ApellidoMaterno = u.ApellidoMaterno,
                        FechaNacimiento = u.FechaNacimiento,
                        Activo = u.Activo,
                        FechaCreacion = u.FechaCreacion,
                        FechaActualizacion = u.FechaActualizacion,
                        Roles = _userManager.GetRolesAsync(u).Result.ToList()
                    })
                    .FirstOrDefaultAsync();

                return HandleResult(user, "Usuario no encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el usuario con ID: {id}");
                return StatusCode(500, "Error interno del servidor al obtener el usuario");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                    return BadRequest(new { Message = "El correo electrónico ya está registrado" });

                var user = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    ApellidoPaterno = model.ApellidoPaterno,
                    ApellidoMaterno = model.ApellidoMaterno,
                    FechaNacimiento = model.FechaNacimiento,
                    FechaCreacion = DateTime.UtcNow,
                    Activo = model.Activo
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                if (model.Roles != null && model.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }
                else
                {
                    // Rol predeterminado si no se especifica ninguno
                    await _userManager.AddToRoleAsync(user, "Usuario");
                }

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new { Message = "Usuario creado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                return StatusCode(500, "Error interno del servidor al crear el usuario");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return NotFound("Usuario no encontrado");

                // Actualizar propiedades básicas
                user.Nombre = model.Nombre ?? user.Nombre;
                user.ApellidoPaterno = model.ApellidoPaterno ?? user.ApellidoPaterno;
                user.ApellidoMaterno = model.ApellidoMaterno ?? user.ApellidoMaterno;
                user.FechaNacimiento = model.FechaNacimiento ?? user.FechaNacimiento;
                user.Activo = model.Activo ?? user.Activo;
                user.FechaActualizacion = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                // Actualizar roles si se proporcionan
                if (model.Roles != null && model.Roles.Any())
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }

                return Ok(new { Message = "Usuario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el usuario con ID: {id}");
                return StatusCode(500, "Error interno del servidor al actualizar el usuario");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return NotFound("Usuario no encontrado");

                // No permitir eliminar el propio usuario
                var currentUserId = User.FindFirst("uid")?.Value;
                if (user.Id.ToString() == currentUserId)
                    return BadRequest("No puedes eliminar tu propio usuario");

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el usuario con ID: {id}");
                return StatusCode(500, "Error interno del servidor al eliminar el usuario");
            }
        }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class CreateUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool Activo { get; set; } = true;
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UpdateUserModel
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool? Activo { get; set; }
        public List<string> Roles { get; set; }
    }
}
