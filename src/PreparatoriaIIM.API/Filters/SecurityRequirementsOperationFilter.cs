using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace PreparatoriaIIM.API.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Obtener los atributos de autorización del controlador y la acción
            var requiredScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Distinct();

            if (requiredScopes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "No autorizado" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Prohibido" });

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [oAuthScheme] = requiredScopes.ToList()
                    }
                };
            }
        }
    }
}
