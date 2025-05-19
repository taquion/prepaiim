using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using PreparatoriaIIM.Infrastructure.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.API.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(
            IConfiguration configuration,
            ILogger<DatabaseHealthCheck> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                              throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT 1";
                    
                    var result = await command.ExecuteScalarAsync(cancellationToken);
                    
                    if (result?.ToString() == "1")
                    {
                        return HealthCheckResult.Healthy("La base de datos está respondiendo correctamente.");
                    }
                    
                    return HealthCheckResult.Unhealthy("La base de datos no devolvió el resultado esperado.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la conexión a la base de datos");
                return HealthCheckResult.Unhealthy("Error al conectar con la base de datos", ex);
            }
        }
    }
}
