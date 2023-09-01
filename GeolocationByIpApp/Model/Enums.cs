namespace GeolocationApp.Model
{
    public static class Enums
    {
        public enum EntryType
        {
            IPAddress,
            DomainName,
            None
        }

        public enum LogLevel
        {
            DetailedLogging,
            Info
        }

        public enum ResponseState
        {
            Success,
            Notification,
            Fail
        }
        public enum HealthServiceType
        {
            ApiService,
            ConfigService,
            InputService,
            LoggingService,
            NotificationService,
            SqlLiteService
        }
    }
}