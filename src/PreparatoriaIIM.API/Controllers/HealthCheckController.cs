using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PreparatoriaIIM.API.Controllers
{
    /// <summary>
    /// Controlador para verificar el estado de salud de la aplicación
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(
            HealthCheckService healthCheckService,
            ILogger<HealthCheckController> logger)
        {
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene el estado de salud de la aplicación
        /// </summary>
        /// <returns>Estado de salud actual</returns>
        /// <response code="200">La aplicación está funcionando correctamente</response>
        /// <response code="503">Uno o más servicios no están disponibles</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetHealth()
        {
            _logger.LogInformation("Solicitud de verificación de salud recibida");
            
            var healthReport = await _healthCheckService.CheckHealthAsync(new HealthCheckContext());
            
            var status = healthReport.Status;
            var isHealthy = status == HealthStatus.Healthy;
            
            var response = new
            {
                status = status.ToString(),
                timestamp = DateTime.UtcNow,
                results = healthReport.Entries.Select(e => new
                {
                    key = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration,
                    exception = e.Value.Exception?.Message,
                    data = e.Value.Data
                }),
                totalDuration = healthReport.TotalDuration
            };
            
            _logger.LogInformation("Resultado de la verificación de salud: {Status}", status);
            
            return isHealthy 
                ? Ok(response) 
                : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
        
        /// <summary>
        /// Verifica si la API está en línea
        /// </summary>
        /// <returns>Mensaje de estado</returns>
        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            _logger.LogInformation("Solicitud de ping recibida");
            return Ok(new { message = "API en línea", timestamp = DateTime.UtcNow });
        }
        
        /// <summary>
        /// Obtiene información de la versión de la API
        /// </summary>
        /// <returns>Información de la versión</returns>
        [HttpGet("version")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetVersion()
        {
            var version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";
            return Ok(new 
            { 
                version,
                name = "Preparatoria IIM API",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }
    }
}
