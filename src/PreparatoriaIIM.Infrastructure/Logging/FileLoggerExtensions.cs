using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace PreparatoriaIIM.Infrastructure.Logging
{
    public static class FileLoggerExtensions
    {
        /// <summary>
        /// Agrega el proveedor de logs a archivos a la aplicación
        /// </summary>
        public static ILoggingBuilder AddFileLogger(
            this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            
            // Registrar el proveedor de configuración
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
                
            // Registrar las opciones de configuración
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<FileLoggerOptions>, FileLoggerOptionsSetup>());
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IOptionsChangeTokenSource<FileLoggerOptions>, 
                    LoggerProviderOptionsChangeTokenSource<FileLoggerOptions, FileLoggerProvider>>());
            
            return builder;
        }

        /// <summary>
        /// Agrega el proveedor de logs a archivos con configuración personalizada
        /// </summary>
        public static ILoggingBuilder AddFileLogger(
            this ILoggingBuilder builder,
            Action<FileLoggerOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
                
            builder.AddFileLogger();
            builder.Services.Configure(configure);
            
            return builder;
        }
    }

    /// <summary>
    /// Configuración de opciones para el proveedor de logs a archivos
    /// </summary>
    internal class FileLoggerOptionsSetup : ConfigureFromConfigurationOptions<FileLoggerOptions>
    {
        public FileLoggerOptionsSetup(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration)
        {
        }
    }
}
