using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.API.HealthChecks;

namespace PreparatoriaIIM.API.Controllers
{
    /// <summary>
    /// Controlador para exponer información de salud y métricas de la aplicación
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IHealthCheckService _healthCheckService;
        private readonly IOptions<HealthCheckServiceOptions> _healthCheckOptions;

        public HealthController(
            ILogger<HealthController> logger,
            IHealthCheckService healthCheckService,
            IOptions<HealthCheckServiceOptions> healthCheckOptions = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            _healthCheckOptions = healthCheckOptions ?? throw new ArgumentNullException(nameof(healthCheckOptions));
        }

        /// <summary>
        /// Obtiene el estado de salud general de la aplicación
        /// </summary>
        /// <returns>Estado de salud de la aplicación</returns>
        /// <response code="200">La aplicación está funcionando correctamente</response>
        /// <response code="503">La aplicación no está funcionando correctamente</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Solicitud de verificación de salud recibida");
            
            var healthReport = await _healthCheckService.CheckHealthAsync();
            
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
        /// Obtiene métricas de rendimiento de la aplicación
        /// </summary>
        /// <returns>Métricas de rendimiento</returns>
        [HttpGet("metrics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMetrics()
        {
            var process = Process.GetCurrentProcess();
            
            // Obtener métricas del proceso
            var metrics = new
            {
                processInfo = new
                {
                    processName = process.ProcessName,
                    startTime = process.StartTime,
                    totalProcessorTime = process.TotalProcessorTime,
                    userProcessorTime = process.UserProcessorTime,
                    privilegedProcessorTime = process.PrivilegedProcessorTime
                },
                memory = new
                {
                    workingSetMB = process.WorkingSet64 / (1024 * 1024),
                    privateMemoryMB = process.PrivateMemorySize64 / (1024 * 1024),
                    virtualMemoryMB = process.VirtualMemorySize64 / (1024 * 1024),
                    pagedMemoryMB = process.PagedMemorySize64 / (1024 * 1024),
                    nonpagedSystemMemoryMB = process.NonpagedSystemMemorySize64 / (1024 * 1024)
                },
                threads = new
                {
                    threadCount = process.Threads.Count,
                    priorityClass = process.PriorityClass.ToString()
                },
                environment = new
                {
                    machineName = Environment.MachineName,
                    osVersion = Environment.OSVersion.ToString(),
                    processorCount = Environment.ProcessorCount,
                    systemPageSize = Environment.SystemPageSize,
                    is64BitProcess = Environment.Is64BitProcess,
                    is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                    currentDirectory = Environment.CurrentDirectory,
                    runtimeVersion = Environment.Version.ToString()
                },
                gc = new
                {
                    totalMemoryKB = GC.GetTotalMemory(false) / 1024,
                    maxGeneration = GC.MaxGeneration,
                    collectionCounts = new
                    {
                        gen0 = GC.CollectionCount(0),
                        gen1 = GC.CollectionCount(1),
                        gen2 = GC.CollectionCount(2)
                    }
                },
                timestamp = DateTime.UtcNow
            };
            
            return Ok(metrics);
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
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
