using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.Infrastructure.Logging
{
    [ProviderAlias("File")]
    public class FileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        private readonly IOptionsMonitor<FileLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers;
        private readonly BlockingCollection<LogEntry> _logEntries;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _processLogsTask;
        private IExternalScopeProvider _scopeProvider = default!;
        private bool _disposed;
        private readonly string _logDirectory;
        private readonly string _logFileBaseName;
        private readonly string _logFileExtension = ".log";
        private readonly int _maxFileSizeBytes;
        private readonly int _maxFilesToRetain;
        private readonly object _fileLock = new object();

        public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, FileLogger>();
            _logEntries = new BlockingCollection<LogEntry>();
            _cancellationTokenSource = new CancellationTokenSource();
            _processLogsTask = Task.Run(ProcessLogs, _cancellationTokenSource.Token);

            // Configuración de los logs
            var settings = _options.CurrentValue;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), 
                settings.LogDirectory.TrimStart('~', '/'));
            _logFileBaseName = settings.FileName ?? "log";
            _maxFileSizeBytes = settings.FileSizeLimitBytes;
            _maxFilesToRetain = settings.RetainedFileCountLimit;

            // Asegurarse de que el directorio de logs exista
            Directory.CreateDirectory(_logDirectory);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => 
                new FileLogger(name, this, GetCurrentConfig));
        }

        internal void QueueLogEntry(LogEntry logEntry)
        {
            if (!_logEntries.IsAddingCompleted)
            {
                try
                {
                    _logEntries.Add(logEntry, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Ignorar, se está cerrando
                }
            }
        }

        private FileLoggerConfiguration GetCurrentConfig() => new()
        {
            LogLevel = _options.CurrentValue.LogLevel,
            LogLevels = _options.CurrentValue.LogLevels,
            EventId = 0
        };

        private async Task ProcessLogs()
        {
            try
            {
                foreach (var logEntry in _logEntries.GetConsumingEnumerable(_cancellationTokenSource.Token))
                {
                    await WriteLogEntryAsync(logEntry);
                }
            }
            catch (OperationCanceledException)
            {
                // Se espera cuando se cancela
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el procesamiento de logs: {ex}");
            }
        }

        private async Task WriteLogEntryAsync(LogEntry logEntry)
        {
            var logMessage = FormatLogEntry(logEntry);
            var logFilePath = GetCurrentLogFilePath();

            try
            {
                lock (_fileLock)
                {
                    // Verificar si el archivo actual supera el tamaño máximo
                    var fileInfo = new FileInfo(logFilePath);
                    if (fileInfo.Exists && fileInfo.Length > _maxFileSizeBytes)
                    {
                        RollLogFiles();
                        logFilePath = GetCurrentLogFilePath();
                    }

                    // Escribir en el archivo
                    File.AppendAllText(logFilePath, logMessage, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de log: {ex}");
                // Opcional: Intentar escribir en un archivo de respaldo o en la consola
                await Console.Error.WriteLineAsync($"[FALLBACK LOG] {logMessage}");
            }
        }

        private string FormatLogEntry(LogEntry logEntry)
        {
            var logLevel = logEntry.Level.ToString().ToUpper().PadRight(5);
            var timestamp = logEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var category = logEntry.Category;
            var message = logEntry.Message;
            var exception = logEntry.Exception != null ? 
                $"\n{logEntry.Exception}" : string.Empty;

            return $"[{timestamp}] {logLevel} [{category}] {message}{exception}\n";
        }

        private string GetCurrentLogFilePath()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(_logDirectory, $"{_logFileBaseName}-{timestamp}{_logFileExtension}");
        }

        private void RollLogFiles()
        {
            try
            {
                var files = Directory.GetFiles(_logDirectory, $"{_logFileBaseName}-*{_logFileExtension}")
                    .OrderByDescending(f => f)
                    .ToArray();

                // Eliminar archivos antiguos si excedemos el límite
                for (int i = _maxFilesToRetain - 1; i < files.Length; i++)
                {
                    try
                    {
                        File.Delete(files[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al eliminar archivo de log antiguo {files[i]}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al rotar archivos de log: {ex.Message}");
            }
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Marcar que no se aceptan más entradas
                    _logEntries.CompleteAdding();
                    
                    // Cancelar el procesamiento de logs
                    _cancellationTokenSource.Cancel();
                    
                    try
                    {
                        // Esperar a que termine de procesar los logs pendientes
                        _processLogsTask.Wait(TimeSpan.FromSeconds(5));
                    }
                    catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                    {
                        // Se espera cuando se cancela
                    }
                    
                    _cancellationTokenSource.Dispose();
                    _logEntries.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                // Marcar que no se aceptan más entradas
                _logEntries.CompleteAdding();
                
                // Cancelar el procesamiento de logs
                _cancellationTokenSource.Cancel();
                
                try
                {
                    // Esperar a que termine de procesar los logs pendientes
                    await _processLogsTask.WaitAsync(TimeSpan.FromSeconds(5));
                }
                catch (OperationCanceledException)
                {
                    // Se espera cuando se cancela
                }
                
                _cancellationTokenSource.Dispose();
                _logEntries.Dispose();
                
                _disposed = true;
            }
            
            GC.SuppressFinalize(this);
        }
    }
}
