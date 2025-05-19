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
    public class RolesController : BaseApiController
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<Usuario> userManager,
            ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _roleManager.Roles
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        NormalizedName = r.NormalizedName
                    })
                    .ToList();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de roles");
                return StatusCode(500, "Error interno del servidor al obtener los roles");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                    return NotFound("Rol no encontrado");

                return Ok(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    NormalizedName = role.NormalizedName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el rol con ID: {id}");
                return StatusCode(500, "Error interno del servidor al obtener el rol");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleModel model)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(model.Name))
                    return BadRequest("El rol ya existe");

                var role = new IdentityRole<Guid>(model.Name);
                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new { Message = "Rol creado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol");
                return StatusCode(500, "Error interno del servidor al crear el rol");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleModel model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                    return NotFound("Rol no encontrado");

                role.Name = model.Name ?? role.Name;
                role.NormalizedName = model.Name?.ToUpper() ?? role.NormalizedName;

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(new { Message = "Rol actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el rol con ID: {id}");
                return StatusCode(500, "Error interno del servidor al actualizar el rol");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                    return NotFound("Rol no encontrado");

                // Verificar si hay usuarios con este rol
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                    return BadRequest("No se puede eliminar el rol porque tiene usuarios asignados");

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el rol con ID: {id}");
                return StatusCode(500, "Error interno del servidor al eliminar el rol");
            }
        }

        [HttpGet("{roleId}/users")]
        public async Task<IActionResult> GetUsersInRole(string roleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                    return NotFound("Rol no encontrado");

                var query = _userManager.Users
                    .Where(u => _userManager.IsInRoleAsync(u, role.Name).Result)
                    .OrderBy(u => u.ApellidoPaterno)
                    .ThenBy(u => u.Nombre);

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
                        Activo = u.Activo
                    })
                    .ToListAsync();

                return HandlePagedResult(users, pageNumber, pageSize, totalItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los usuarios del rol con ID: {roleId}");
                return StatusCode(500, "Error interno del servidor al obtener los usuarios del rol");
            }
        }
    }

    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }

    public class CreateRoleModel
    {
        public string Name { get; set; }
    }

    public class UpdateRoleModel
    {
        public string Name { get; set; }
    }
}
