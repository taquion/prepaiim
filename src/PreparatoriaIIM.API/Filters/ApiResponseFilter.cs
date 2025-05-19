using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PreparatoriaIIM.API.Models;
using System.Linq;

namespace PreparatoriaIIM.API.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result == null) return;

            if (context.Result is ObjectResult objectResult)
            {
                // Si ya es un ApiResponse, no hacer nada
                if (objectResult.Value != null && objectResult.Value.GetType().IsGenericType && 
                    objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    return;
                }

                // Si es un resultado exitoso (código 2xx)
                if (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
                {
                    var responseType = typeof(ApiResponse<>).MakeGenericType(
                        objectResult.Value?.GetType() ?? typeof(object));
                    
                    var successResponse = typeof(ApiResponse<object>)
                        .GetMethod(nameof(ApiResponse<object>.SuccessResponse), 
                            new[] { typeof(object), typeof(string) })
                        .MakeGenericMethod(objectResult.Value?.GetType() ?? typeof(object));

                    var response = successResponse.Invoke(null, new[] { objectResult.Value, null });
                    
                    context.Result = new ObjectResult(response)
                    {
                        StatusCode = objectResult.StatusCode
                    };
                }
            }
            else if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                // Manejar errores de validación de modelo
                if (badRequestObjectResult.Value is SerializableError errors)
                {
                    var errorMessages = errors.SelectMany(e => 
                        ((string[])e.Value).Select(v => $"{e.Key}: {v}"));
                    
                    var response = ApiResponse<object>.ErrorResponse("Error de validación", errorMessages);
                    context.Result = new BadRequestObjectResult(response);
                }
                else
                {
                    var response = ApiResponse<object>.ErrorResponse(badRequestObjectResult.Value?.ToString());
                    context.Result = new BadRequestObjectResult(response);
                }
            }
            else if (context.Result is NotFoundResult || context.Result is NotFoundObjectResult)
            {
                var notFoundResult = context.Result as NotFoundObjectResult;
                var response = ApiResponse<object>.ErrorResponse(
                    notFoundResult?.Value?.ToString() ?? "Recurso no encontrado");
                context.Result = new NotFoundObjectResult(response);
            }
        }
    }
}
