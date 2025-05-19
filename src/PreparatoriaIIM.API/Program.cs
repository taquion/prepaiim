using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using PreparatoriaIIM.API.Extensions;
using PreparatoriaIIM.API.Filters;
using PreparatoriaIIM.API.Middleware;
using PreparatoriaIIM.Infrastructure;
using PreparatoriaIIM.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddInfrastructure(builder.Configuration);

// Configuración de caché en memoria
builder.Services.AddMemoryCache();

// Configuración de autenticación JWT
builder.Services.AddJwtBearerAuthentication(builder.Configuration);

// Configuración de controladores con el filtro de respuesta API
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Configurar el manejador de excepciones personalizado
app.UseCustomExceptionHandler();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Configuración de Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Preparatoria IIM API v1");
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthUsePkce();
        c.OAuthScopeSeparator(" ");
    });

    // Aplicar migraciones automáticamente en desarrollo
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            
            // Aquí puedes agregar datos iniciales si es necesario
            // await SeedData.Initialize(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
        }
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

// Middleware para rastrear usuarios en línea
app.UseOnlineUsersTracking();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
