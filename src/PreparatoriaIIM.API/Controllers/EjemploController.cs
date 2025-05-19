using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PreparatoriaIIM.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PreparatoriaIIM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope("access_as_user")]
    public class EjemploController : BaseApiController
    {
        private static readonly List<string> _ejemplos = new List<string>
        {
            "Ejemplo 1",
            "Ejemplo 2",
            "Ejemplo 3"
        };

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            // Usando el método de ayuda del controlador base
            return HandleResult(_ejemplos, "No se encontraron ejemplos");
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            if (id < 0 || id >= _ejemplos.Count)
                return NotFound("Ejemplo no encontrado");

            // La respuesta se envuelve automáticamente en un ApiResponse<T>
            return Ok(_ejemplos[id]);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Crear([FromBody] string ejemplo)
        {
            if (string.IsNullOrWhiteSpace(ejemplo))
                return BadRequest("El ejemplo no puede estar vacío");

            _ejemplos.Add(ejemplo);
            
            // Retornando un objeto anónimo que se envuelve automáticamente
            return CreatedAtAction(nameof(ObtenerPorId), new { id = _ejemplos.Count - 1 }, ejemplo);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Actualizar(int id, [FromBody] string ejemplo)
        {
            if (id < 0 || id >= _ejemplos.Count)
                return NotFound("Ejemplo no encontrado");

            if (string.IsNullOrWhiteSpace(ejemplo))
                return BadRequest("El ejemplo no puede estar vacío");

            _ejemplos[id] = ejemplo;
            
            // Retornando un mensaje de éxito
            return Ok("Ejemplo actualizado correctamente");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Eliminar(int id)
        {
            if (id < 0 || id >= _ejemplos.Count)
                return NotFound("Ejemplo no encontrado");

            _ejemplos.RemoveAt(id);
            
            // Retornando un mensaje de éxito
            return NoContent();
        }
    }
}
