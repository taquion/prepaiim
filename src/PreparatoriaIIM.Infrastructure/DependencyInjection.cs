using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PreparatoriaIIM.Infrastructure.Data;

namespace PreparatoriaIIM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración de la base de datos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Configuración de Identity
            services.AddIdentity<Usuario, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configuración de Azure Blob Storage
            services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorage"));
            services.AddScoped<IFileStorageService, AzureBlobStorageService>();

            // Configuración de Azure AD
            services.Configure<AzureAdConfig>(configuration.GetSection("AzureAd"));
            services.AddScoped<IAzureAdService, AzureAdService>();

            // Configuración del servicio de correo electrónico
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            // Configuración del servicio de informes PDF
            services.AddScoped<IPdfReportService, PdfReportService>();

            // Configuración de CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins(configuration["AllowedOrigins"]?.Split(';') ?? Array.Empty<string>())
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            // Configuración de CORS
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder => builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // Configuración de Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Preparatoria IIM API", Version = "v1" });
                
                // Configuración de autenticación JWT en Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                                  "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                                  "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // Configuración de operaciones
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // Configuración de servicios de infraestructura
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPdfReportService, PdfReportService>();
            services.AddScoped<IStorageService, AzureBlobStorageService>();
            
            // Configuración del sistema de logging personalizado
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.AddFile();
                
                // Configuración de niveles de log por categoría
                logging.AddFilter("Microsoft", LogLevel.Warning);
                logging.AddFilter("System", LogLevel.Warning);
                logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
                logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            });
            
            // Configuración de opciones del logger de archivo
            services.Configure<FileLoggerOptions>(configuration.GetSection("Logging:File"));
        
            // Configurar cliente de Azure Blob Storage
            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("AzureStorage") ?? 
                                     configuration["AzureStorage:ConnectionString"];
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    var logger = provider.GetRequiredService<ILogger<AzureBlobStorageService>>();
                    logger.LogWarning("No se encontró la cadena de conexión de Azure Storage. Se usará la configuración por defecto.");
                    
                    var accountName = configuration["AzureStorage:AccountName"] ?? "stiimprepaprod64110";
                    var accountKey = configuration["AzureStorage:AccountKey"] ?? "[TU_ACCOUNT_KEY]";
                    
                    var storageCredentials = new Azure.Storage.StorageSharedKeyCredential(accountName, accountKey);
                    var blobUri = new Uri($"https://{accountName}.blob.core.windows.net");
                    return new BlobServiceClient(blobUri, storageCredentials);
                }
                
                return new BlobServiceClient(connectionString);
            });
            
            // Configuración de EmailSettings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Configuración de controladores con JSON serialization
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            return services;
        }
    }

    // Clase auxiliar para la configuración de seguridad de Swagger
    internal class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var requiredScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Concat(context.MethodInfo.DeclaringType
                    .GetCustomAttributes(true)
                    .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                    .Select(attr => attr.Policy))
                .Distinct();

            if (requiredScopes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

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
