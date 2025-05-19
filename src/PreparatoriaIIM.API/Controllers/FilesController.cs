using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PreparatoriaIIM.Infrastructure.Services;

namespace PreparatoriaIIM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope("access_as_user")]
    public class FilesController : ControllerBase
    {
        private readonly AzureBlobStorageService _blobStorage;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FilesController> _logger;

        public FilesController(AzureBlobStorageService blobStorage, IConfiguration configuration, ILogger<FilesController> logger)
        {
            _blobStorage = blobStorage;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string containerName = "documents")
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado ningún archivo.");

            try
            {
                using var stream = file.OpenReadStream();
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fileUrl = await _blobStorage.UploadFileAsync(containerName, fileName, stream);
                
                return Ok(new { FileUrl = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el archivo");
                return StatusCode(500, "Error interno al cargar el archivo");
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName, string containerName = "documents")
        {
            try
            {
                var stream = await _storageService.DownloadFileAsync(containerName, fileName);
                
                if (stream == null)
                    return NotFound("Archivo no encontrado");
                
                var contentType = GetContentType(fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar el archivo {FileName}", fileName);
                return StatusCode(500, ApiResponse<object>.Failure("Error interno al descargar el archivo"));
            }
        }

        [HttpGet("url/{fileName}")]
        [ProducesResponseType(typeof(ApiResponse<FileUrlResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetFileUrl(string fileName, string containerName = null, int expiryInHours = 1)
        {
            try
            {
                var targetContainerName = string.IsNullOrEmpty(containerName) ? DefaultContainerName : containerName.ToLower();
                var expiryTime = TimeSpan.FromHours(expiryInHours > 0 ? expiryInHours : 1);
                
                var fileUrl = await _storageService.GetFileUrlAsync(targetContainerName, fileName, expiryTime);
                
                if (fileUrl == null)
                    return NotFound(ApiResponse<object>.Failure("Archivo no encontrado"));
                    
                _logger.LogInformation("URL generada para el archivo {FileName} del contenedor {ContainerName}", 
                    fileName, targetContainerName);
                
                var response = new FileUrlResponse
                {
                    FileName = fileName,
                    FileUrl = fileUrl.ToString(),
                    ExpiresAt = DateTime.UtcNow.Add(expiryTime)
                };
                
                return Ok(ApiResponse<FileUrlResponse>.Success(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar la URL para el archivo {FileName}", fileName);
                return StatusCode(500, ApiResponse<object>.Failure("Error al generar la URL del archivo"));
            }
        }

        [HttpDelete("{fileName}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteFile(string fileName, string containerName = null)
        {
            try
            {
                await _blobStorage.DeleteFileAsync(containerName, fileName);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el archivo {fileName}");
                return StatusCode(500, "Error interno al eliminar el archivo");
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
