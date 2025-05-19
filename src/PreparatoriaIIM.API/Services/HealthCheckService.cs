using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.Application.Common.Interfaces;

namespace PreparatoriaIIM.API.Services
{
    public class HealthCheckService : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;
        private readonly ILogger<HealthCheckService> _logger;
        private readonly string _connectionString;

        public HealthCheckService(
            IConfiguration configuration,
            IStorageService storageService,
            ILogger<HealthCheckService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? 
                              throw new InvalidOperationException("No se encontró la cadena de conexión predeterminada");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>();
            var healthCheckResults = new List<HealthCheckResult>();

            try
            {
                // Verificar conexión a la base de datos
                var dbCheck = await CheckDatabaseConnectionAsync(cancellationToken);
                healthCheckResults.Add(dbCheck);
                data.Add("Database", dbCheck.Status.ToString());

                // Verificar almacenamiento
                var storageCheck = await CheckStorageConnectionAsync(cancellationToken);
                healthCheckResults.Add(storageCheck);
                data.Add("Storage", storageCheck.Status.ToString());

                // Verificar uso de memoria
                var memoryCheck = CheckMemoryUsage();
                healthCheckResults.Add(memoryCheck);
                data.Add("MemoryUsage", memoryCheck.Status.ToString());
                data.Add("WorkingSetMB", Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024);

                // Determinar el estado general
                var overallStatus = healthCheckResults.Max(r => r.Status);
                var description = overallStatus == HealthStatus.Healthy 
                    ? "La aplicación está funcionando correctamente" 
                    : "Se han detectado problemas en uno o más servicios";

                return new HealthCheckResult(
                    overallStatus,
                    description,
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la verificación de salud");
                return new HealthCheckResult(
                    HealthStatus.Unhealthy, 
                    "Error al verificar el estado de salud de la aplicación",
                    ex,
                    data);
            }
        }

        private async Task<HealthCheckResult> CheckDatabaseConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                await command.ExecuteScalarAsync(cancellationToken);
                
                return HealthCheckResult.Healthy("La conexión a la base de datos es estable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la conexión a la base de datos");
                return HealthCheckResult.Unhealthy("Error al conectar con la base de datos", ex);
            }
        }

        private async Task<HealthCheckResult> CheckStorageConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Intentar realizar una operación simple de almacenamiento
                var testFileName = $"healthcheck-{Guid.NewGuid()}.txt";
                var testContent = "Health check test";
                
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testContent));
                await _storageService.UploadFileAsync("healthchecks", testFileName, stream);
                
                // Limpiar después de la prueba
                await _storageService.DeleteFileAsync("healthchecks", testFileName);
                
                return HealthCheckResult.Healthy("La conexión al almacenamiento es estable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la conexión al almacenamiento");
                return HealthCheckResult.Unhealthy("Error al conectar con el servicio de almacenamiento", ex);
            }
        }

        private HealthCheckResult CheckMemoryUsage()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var workingSetMB = process.WorkingSet64 / 1024 / 1024;
                var maxMemoryMB = 500; // Límite de memoria en MB

                if (workingSetMB > maxMemoryMB * 0.9) // 90% del límite
                {
                    return HealthCheckResult.Degraded($"El uso de memoria es alto: {workingSetMB}MB");
                }

                return HealthCheckResult.Healthy($"Uso de memoria: {workingSetMB}MB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el uso de memoria");
                return HealthCheckResult.Unhealthy("No se pudo verificar el uso de memoria", ex);
            }
        }
    }
}
