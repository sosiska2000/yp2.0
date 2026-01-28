using RestAPI.Connect;
using RestAPI.Models;
using System;
using System.IO;
using System.Linq;

namespace RestAPI.Services
{
    public class LogiOshibokService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _logDirectory;

        public LogiOshibokService(ApplicationDbContext context)
        {
            _context = context;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

  
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public IQueryable<LogOshibki> GetAll(DateTime? fromDate = null, DateTime? toDate = null, string? module = null, string? search = null)
        {
            var query = _context.LogiOshibok.AsQueryable();

     
            if (fromDate.HasValue)
            {
                query = query.Where(l => l.DataVremia >= fromDate.Value);
            }

    
            if (toDate.HasValue)
            {
                query = query.Where(l => l.DataVremia <= toDate.Value);
            }

 
            if (!string.IsNullOrEmpty(module))
            {
                query = query.Where(l => l.Module.Contains(module));
            }


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.Soobshchenie.Contains(search));
            }


            return query.OrderByDescending(l => l.DataVremia);
        }

        public LogOshibki? GetById(int id)
        {
            return _context.LogiOshibok.Find(id);
        }

        public void LogError(string module, string message, int? userId = null, Exception? exception = null)
        {
            try
            {

                var logEntry = new LogOshibki
                {
                    Module = module,
                    Soobshchenie = message,
                    PolzovatelId = userId,
                    DataVremia = DateTime.Now
                };

                _context.LogiOshibok.Add(logEntry);
                _context.SaveChanges();


                WriteToFileLog(module, message, exception);
            }
            catch (Exception ex)
            {
    
                WriteToFileLog("LogiOshibokService", $"Ошибка записи лога: {ex.Message}", ex);
            }
        }

        public void LogWarning(string module, string message, int? userId = null)
        {
            LogError(module, $"WARNING: {message}", userId);
        }

        public void LogInfo(string module, string message, int? userId = null)
        {
            LogError(module, $"INFO: {message}", userId);
        }

        private void WriteToFileLog(string module, string message, Exception? exception = null)
        {
            try
            {
   
                var logFileName = $"errors_{DateTime.Now:yyyy-MM-dd}.log";
                var logFilePath = Path.Combine(_logDirectory, logFileName);


                var logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{module}] {message}";

                if (exception != null)
                {
                    logLine += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
                }

                logLine += "\n" + new string('-', 80) + "\n";


                File.AppendAllText(logFilePath, logLine);
            }
            catch
            {
                
            }
        }

        public void ClearOldLogs(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);


                var oldLogs = _context.LogiOshibok.Where(l => l.DataVremia < cutoffDate).ToList();

                if (oldLogs.Any())
                {
                    _context.LogiOshibok.RemoveRange(oldLogs);
                    _context.SaveChanges();
                }

       
                var logFiles = Directory.GetFiles(_logDirectory, "*.log");
                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("LogiOshibokService", $"Ошибка очистки старых логов: {ex.Message}");
            }
        }

        public int GetLogCount(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.LogiOshibok.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.DataVremia >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l => l.DataVremia <= toDate.Value);
            }

            return query.Count();
        }

        public void DeleteLog(int id)
        {
            var log = _context.LogiOshibok.Find(id);
            if (log != null)
            {
                _context.LogiOshibok.Remove(log);
                _context.SaveChanges();
            }
        }
    }
}