using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.API.HealthChecks
{
    public class BasicHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            // Esta es una implementación básica que siempre devuelve un estado saludable
            // En una implementación real, podrías verificar la conexión a la base de datos,
            // servicios externos, etc.
            return Task.FromResult(
                HealthCheckResult.Healthy("La aplicación está funcionando correctamente"));
        }
    }
}
