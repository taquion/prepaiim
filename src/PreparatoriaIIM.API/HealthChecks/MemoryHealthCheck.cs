using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.API.HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly ILogger<MemoryHealthCheck> _logger;
        private readonly long _warningThresholdMB;
        private readonly long _errorThresholdMB;

        public MemoryHealthCheck(
            IConfiguration configuration,
            ILogger<MemoryHealthCheck> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Umbrales en MB (predeterminados si no se especifican)
            _warningThresholdMB = configuration.GetValue<long>("HealthChecks:Memory:WarningThresholdMB", 500);
            _errorThresholdMB = configuration.GetValue<long>("HealthChecks:Memory:ErrorThresholdMB", 800);
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var workingSetMB = process.WorkingSet64 / (1024 * 1024);
                var privateMemoryMB = process.PrivateMemorySize64 / (1024 * 1024);
                var virtualMemoryMB = process.VirtualMemorySize64 / (1024 * 1024);
                var threadCount = process.Threads.Count;
                
                var data = new Dictionary<string, object>
                {
                    { "working_set_mb", workingSetMB },
                    { "private_memory_mb", privateMemoryMB },
                    { "virtual_memory_mb", virtualMemoryMB },
                    { "thread_count", threadCount },
                    { "warning_threshold_mb", _warningThresholdMB },
                    { "error_threshold_mb", _errorThresholdMB }
                };
                
                // Verificar si el uso de memoria supera los umbrales
                if (workingSetMB >= _errorThresholdMB)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"El uso de memoria es crítico: {workingSetMB}MB (Umbral: {_errorThresholdMB}MB)",
                        null,
                        data));
                }
                
                if (workingSetMB >= _warningThresholdMB)
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"El uso de memoria es alto: {workingSetMB}MB (Umbral: {_warningThresholdMB}MB)",
                        null,
                        data));
                }
                
                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Uso de memoria normal: {workingSetMB}MB",
                    data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el uso de memoria");
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Error al verificar el uso de memoria",
                    ex));
            }
        }
    }
}
