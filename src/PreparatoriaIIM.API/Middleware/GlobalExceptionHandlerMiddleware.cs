using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.API.Models;

namespace PreparatoriaIIM.API.Middleware
{
    /// <summary>
    /// Middleware para el manejo global de excepciones
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            var errorResponse = new ApiResponse<object>
            {
                Success = false
            };

            switch (exception)
            {
                case ApplicationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                    _logger.LogWarning(ex, "Error de aplicación: {Message}", ex.Message);
                    break;
                    
                case UnauthorizedAccessException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "No autorizado";
                    _logger.LogWarning(ex, "Intento de acceso no autorizado: {Message}", ex.Message);
                    break;
                    
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = "Recurso no encontrado";
                    _logger.LogWarning(ex, "Recurso no encontrado: {Message}", ex.Message);
                    break;
                    
                case InvalidOperationException ex:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Message = "Operación no válida";
                    _logger.LogError(ex, "Operación no válida: {Message}", ex.Message);
                    break;
                    
                case ArgumentException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = string.IsNullOrEmpty(ex.Message) ? "Parámetro no válido" : ex.Message;
                    _logger.LogWarning(ex, "Error de argumento: {Message}", ex.Message);
                    break;
                    
                case TimeoutException ex:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Message = "Tiempo de espera agotado";
                    _logger.LogError(ex, "Tiempo de espera agotado: {Message}", ex.Message);
                    break;
                    
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Error interno del servidor";
                    _logger.LogError(exception, "Error no manejado: {Message}", exception.Message);
                    break;
            }

            // No exponer detalles del error en producción
            if (response.StatusCode >= 500)
            {
                errorResponse.Message = "Ha ocurrido un error inesperado. Por favor, inténtelo de nuevo más tarde.";
                _logger.LogError(exception, "Error interno del servidor: {Message}", exception.Message);
            }

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            
            await response.WriteAsync(result);
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
