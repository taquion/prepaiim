using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Infrastructure.Logging
{
    public class FileLoggerOptions
    {
        /// <summary>
        /// Nivel de log mínimo por defecto
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Niveles de log por categoría
        /// </summary>
        public IDictionary<string, LogLevel> LogLevels { get; set; } = new Dictionary<string, LogLevel>();

        /// <summary>
        /// Directorio donde se guardarán los archivos de log
        /// </summary>
        public string LogDirectory { get; set; } = "Logs";

        /// <summary>
        /// Nombre base del archivo de log (sin extensión)
        /// </summary>
        public string FileName { get; set; } = "log";

        /// <summary>
        /// Tamaño máximo de archivo en bytes antes de rotar
        /// </summary>
        public int FileSizeLimitBytes { get; set; } = 5 * 1024 * 1024; // 5MB

        /// <summary>
        /// Número máximo de archivos de log a conservar
        /// </summary>
        public int RetainedFileCountLimit { get; set; } = 5;

        /// <summary>
        /// Indica si se deben incluir los scopes en los logs
        /// </summary>
        public bool IncludeScopes { get; set; } = true;
    }

    public class FileLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public IDictionary<LogLevel, int> LogLevels { get; set; } = new Dictionary<LogLevel, int>
        {
            [LogLevel.Trace] = 0,
            [LogLevel.Debug] = 1,
            [LogLevel.Information] = 2,
            [LogLevel.Warning] = 3,
            [LogLevel.Error] = 4,
            [LogLevel.Critical] = 5,
            [LogLevel.None] = 6
        };
        public int EventId { get; set; } = 0;
    }
}
