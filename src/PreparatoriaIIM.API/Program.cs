using Microsoft.OpenApi.Models;
using PreparatoriaIIM.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddInfrastructure(builder.Configuration);

// Configuración básica de controladores
builder.Services.AddControllers();

// Configuración de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PreparatoriaIIM API", Version = "v1" });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuración de la aplicación
var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PreparatoriaIIM.API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

// Middleware para rastrear usuarios en línea
app.UseOnlineUsersTracking();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check básico
app.MapHealthChecks("/health");

// Configurar el manejador de excepciones personalizado
app.UseCustomExceptionHandler();

// Iniciar la aplicación
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
