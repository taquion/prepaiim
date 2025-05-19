using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.API.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly ILogger<StorageHealthCheck> _logger;

        public StorageHealthCheck(
            IConfiguration configuration,
            ILogger<StorageHealthCheck> logger)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"] ?? 
                              throw new ArgumentNullException(nameof(configuration));
            _containerName = configuration["AzureStorage:ContainerName"] ?? "health-checks";
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_connectionString);
                
                // Verificar que el servicio de almacenamiento esté disponible
                await blobServiceClient.GetAccountInfoAsync(cancellationToken);
                
                // Verificar que el contenedor exista o pueda ser creado
                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                
                // Realizar una operación de escritura/lectura de prueba
                var testBlobName = $"healthcheck-{Guid.NewGuid()}.txt";
                var testContent = "Health check test";
                
                // Subir un blob de prueba
                var blobClient = containerClient.GetBlobClient(testBlobName);
                using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(testContent)))
                {
                    await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
                }
                
                // Leer el blob de prueba
                var download = await blobClient.DownloadContentAsync(cancellationToken);
                var downloadedContent = download.Value.Content.ToString();
                
                // Eliminar el blob de prueba
                await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
                
                // Verificar que el contenido sea el esperado
                if (downloadedContent == testContent)
                {
                    return HealthCheckResult.Healthy("El almacenamiento de Azure Blob está funcionando correctamente.");
                }
                
                return HealthCheckResult.Unhealthy("El contenido del blob de prueba no coincide con el esperado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el almacenamiento de Azure Blob");
                return HealthCheckResult.Unhealthy("Error al verificar el almacenamiento de Azure Blob", ex);
            }
        }
    }
}
