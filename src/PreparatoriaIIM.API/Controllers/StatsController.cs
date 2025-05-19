using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace PreparatoriaIIM.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : BaseApiController
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<StatsController> _logger;
        private const string OnlineUsersCacheKey = "OnlineUsers";

        public StatsController(
            IMemoryCache cache,
            ILogger<StatsController> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("online-users")]
        public IActionResult GetOnlineUsers()
        {
            try
            {
                if (_cache.TryGetValue(OnlineUsersCacheKey, out IEnumerable<string> onlineUserIds))
                {
                    _logger.LogInformation("Usuarios en línea obtenidos exitosamente");
                    return Ok(new
                    {
                        Count = onlineUserIds.Count(),
                        UserIds = onlineUserIds
                    });
                }

                _logger.LogInformation("No hay usuarios en línea");
                return Ok(new
                {
                    Count = 0,
                    UserIds = Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios en línea");
                return StatusCode(500, new { message = "Error al obtener los usuarios en línea" });
            }
        }

        [HttpGet("system-info")]
        public IActionResult GetSystemInfo()
        {
            try
            {
                var info = new
                {
                    ServerName = Environment.MachineName,
                    OS = Environment.OSVersion.ToString(),
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Processors = Environment.ProcessorCount,
                    Memory = GC.GetTotalMemory(forceFullCollection: false),
                    Uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
                };

                _logger.LogInformation("Información del sistema obtenida exitosamente");
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del sistema");
                return StatusCode(500, new { message = "Error al obtener la información del sistema" });
            }
        }
    }
}
