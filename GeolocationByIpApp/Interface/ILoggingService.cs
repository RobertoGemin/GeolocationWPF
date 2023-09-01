using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Interface
{
    public interface ILoggingService
    {
        void Log(string message, LogLevel level = LogLevel.Info);
    }
}