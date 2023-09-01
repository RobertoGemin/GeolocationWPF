using System;
using System.Configuration;
using System.IO;
using GeolocationApp.Interface;
using GeolocationApp.Methods;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Service
{
    public class LoggingService : ILoggingService
    {
        private readonly bool _fullLoggingEnabled;
        private readonly string _logDirectory;
        private readonly string _username;
        public IHealthService HealthService;
        public LoggingService(IHealthService healthService, string user = "example_user")
        {
            HealthService = healthService;
            _logDirectory = GetPath.GetFullPath();
            Directory.CreateDirectory(_logDirectory);

            var fullLoggingEnabledValue = ConfigurationManager.AppSettings["FullLoggingEnabled"];
            _fullLoggingEnabled = !string.IsNullOrEmpty(fullLoggingEnabledValue) && bool.Parse(fullLoggingEnabledValue);

            _username = user;
        }


        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (!_fullLoggingEnabled && level != LogLevel.DetailedLogging) return;

            try
            {
                var logFileName = $"{DateTime.Now:yyyy-MM-dd}-[{_username}].txt";
                var logFilePath = Path.Combine(_logDirectory, logFileName);
                File.AppendAllText(logFilePath, $"\n{DateTime.Now:HH: mm:ss.ffffff} \nType:{level}\n{message}\n");
                var dashes = new string('-', 50);
                File.AppendAllText(logFilePath, dashes);
            }
            catch
            {
                HealthService.UpdateHealthState(HealthServiceType.LoggingService, ResponseState.Fail);
            }
        }
    }
}