using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.Application.Common.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PreparatoriaIIM.Infrastructure.Services;

public class AzureBlobStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private const string DefaultAccountName = "stiimprepaprod64110";
    private const string DefaultAccountKey = "[TU_ACCOUNT_KEY]"; // En producción, usa Azure Key Vault

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        var connectionString = _configuration.GetConnectionString("AzureStorage") ?? 
                             _configuration["AzureStorage:ConnectionString"];
        
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("No se encontró la cadena de conexión de Azure Storage. Se usará la configuración por defecto.");
            var accountName = _configuration["AzureStorage:AccountName"] ?? DefaultAccountName;
            var accountKey = _configuration["AzureStorage:AccountKey"] ?? DefaultAccountKey;
            
            var storageCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            var blobUri = new Uri($"https://{accountName}.blob.core.windows.net");
            _blobServiceClient = new BlobServiceClient(blobUri, storageCredentials);
        }
        else
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
    }

    public async Task<Uri> UploadFileAsync(string containerName, string fileName, Stream fileStream)
    {
        if (string.IsNullOrEmpty(containerName))
            throw new ArgumentException("El nombre del contenedor no puede estar vacío", nameof(containerName));
            
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(fileName));
            
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("El archivo no puede estar vacío", nameof(fileStream));

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            await containerClient.CreateIfNotExistsAsync();
            
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
            
            _logger.LogInformation("Archivo {FileName} subido exitosamente al contenedor {ContainerName}", 
                fileName, containerName);
                
            return blobClient.Uri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir el archivo {FileName} al contenedor {ContainerName}", 
                fileName, containerName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        if (string.IsNullOrEmpty(containerName))
            throw new ArgumentException("El nombre del contenedor no puede estar vacío", nameof(containerName));
            
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(fileName));

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);
            
            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("El archivo {FileName} no existe en el contenedor {ContainerName}", 
                    fileName, containerName);
                return null;
            }
            
            var response = await blobClient.DownloadAsync();
            _logger.LogInformation("Archivo {FileName} descargado exitosamente del contenedor {ContainerName}", 
                fileName, containerName);
                
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar el archivo {FileName} del contenedor {ContainerName}", 
                fileName, containerName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string containerName, string fileName)
    {
        if (string.IsNullOrEmpty(containerName))
            throw new ArgumentException("El nombre del contenedor no puede estar vacío", nameof(containerName));
            
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(fileName));

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);
            
            var result = await blobClient.DeleteIfExistsAsync();
            
            if (result.Value)
            {
                _logger.LogInformation("Archivo {FileName} eliminado exitosamente del contenedor {ContainerName}", 
                    fileName, containerName);
            }
            else
            {
                _logger.LogWarning("No se pudo eliminar el archivo {FileName} del contenedor {ContainerName} porque no existe", 
                    fileName, containerName);
            }
            
            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el archivo {FileName} del contenedor {ContainerName}", 
                fileName, containerName);
            throw;
        }
    }
    
    public async Task<Uri> GetFileUrlAsync(string containerName, string fileName, TimeSpan? expiryTime = null)
    {
        if (string.IsNullOrEmpty(containerName))
            throw new ArgumentException("El nombre del contenedor no puede estar vacío", nameof(containerName));
            
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(fileName));

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);
            
            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("No se puede generar URL para el archivo {FileName} porque no existe en el contenedor {ContainerName}", 
                    fileName, containerName);
                return null;
            }
            
            // Configurar el token SAS
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b", // b para blob, c para contenedor
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime ?? TimeSpan.FromHours(1))
            };
            
            // Permisos de lectura
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            
            // Obtener la clave de la cuenta de almacenamiento
            var accountName = _configuration["AzureStorage:AccountName"] ?? DefaultAccountName;
            var accountKey = _configuration["AzureStorage:AccountKey"] ?? DefaultAccountKey;
            
            var storageSharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
            var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();
            
            // Construir la URL con el token SAS
            var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = Azure.Storage.Sas.SasQueryParameters.Parse(sasToken)
            };
            
            _logger.LogInformation("URL generada para el archivo {FileName} del contenedor {ContainerName}", 
                fileName, containerName);
                
            return blobUriBuilder.ToUri();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar la URL para el archivo {FileName} del contenedor {ContainerName}", 
                fileName, containerName);
            throw;
        }
    }
}
