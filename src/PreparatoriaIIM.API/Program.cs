using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PreparatoriaIIM.API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
