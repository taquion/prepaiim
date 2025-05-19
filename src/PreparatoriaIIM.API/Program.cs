using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "PreparatoriaIIM API", 
        Version = "v1", 
        Description = "¡API desplegada exitosamente en Azure!" 
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PreparatoriaIIM API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// Configurar ruta base si es necesario
var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
if (!string.IsNullOrEmpty(basePath))
{
    app.UsePathBase(basePath);
}
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
