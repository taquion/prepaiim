using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.Infrastructure.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _name;
        private readonly Func<FileLoggerConfiguration> _getCurrentConfig;
        private readonly FileLoggerProvider _provider;
        private readonly string _categoryName;

        public FileLogger(
            string categoryName,
            FileLoggerProvider provider,
            Func<FileLoggerConfiguration> getCurrentConfig)
        {
            _categoryName = categoryName;
            _provider = provider;
            _getCurrentConfig = getCurrentConfig;
            _name = categoryName.Split('.')[^1];
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) =>
            _getCurrentConfig().LogLevels.ContainsKey(logLevel) &&
            _getCurrentConfig().LogLevels[logLevel] <= _getCurrentConfig().LogLevel;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var config = _getCurrentConfig();
            if (config.EventId == 0 || config.EventId == eventId.Id)
            {
                var message = formatter(state, exception);
                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.UtcNow,
                    Level = logLevel,
                    Category = _categoryName,
                    Message = message,
                    Exception = exception
                };

                _provider.QueueLogEntry(logEntry);
            }
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
    }
}
