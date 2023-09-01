using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GeolocationApp.Interface;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Service
{
    public class ConfigSettingService : IConfigSettingService
    {
        public enum ImportanceLevel
        {
            High,
            Low
        }

        public ILoggingService LoggingService;


        public INotificationService NotificationService;

        public IHealthService HealthService;

        public ConfigSettingService(INotificationService notificationService, ILoggingService loggingService, IHealthService healthService)
        {
            NotificationService = notificationService;
            LoggingService = loggingService;
            HealthService = healthService;
        }

        public bool CheckAppConfigKeys()
        {
            var keyImportance = GetKeyWithKeyImportance();
            var missingKeys = FindMissingKeys(keyImportance);
            var missingImportantKeys = GetMissingKeys(keyImportance, missingKeys, ImportanceLevel.High);
            var missingLessImportantKeys = GetMissingKeys(keyImportance, missingKeys, ImportanceLevel.Low);

            if (missingImportantKeys == string.Empty && missingLessImportantKeys == string.Empty)
            {
                NotificationService.Success("Important setting information available");
                return true;
            }

            if (missingImportantKeys == string.Empty)
            {
                NotificationService.Notification("Important setting information available");
            }
            else
            {
                NotificationService.Fail("Important setting information is missing");
                LoggingService.Log($"missingImportantKeys = {missingImportantKeys} \n " +
                                   $"missingLessImportantKeys  = {missingLessImportantKeys} \n");
            }
            HealthService.UpdateHealthState(HealthServiceType.ConfigService, ResponseState.Fail);

            return false;
        }

        public Dictionary<string, ImportanceLevel> GetKeyWithKeyImportance()
        {
            var keyImportance = new Dictionary<string, ImportanceLevel>
            {
                { "ApiKey", ImportanceLevel.High },
                { "ApiCallThreshold", ImportanceLevel.High },
                { "ApiUrl", ImportanceLevel.High },
                { "ApiUrlBalance", ImportanceLevel.High },
                { "EndpointUrl", ImportanceLevel.High },
                { "FullLoggingEnabled", ImportanceLevel.Low },
                { "UseBasePath", ImportanceLevel.High },
                { "BasePath", ImportanceLevel.High },
                { "SpecificPath", ImportanceLevel.Low }
            };
            return keyImportance;
        }


        private List<string> FindMissingKeys(Dictionary<string, ImportanceLevel> keyImportance)
        {
            return keyImportance
                .Where(kvp => ConfigurationManager.AppSettings[kvp.Key] == null)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        private string GetMissingKeys(Dictionary<string, ImportanceLevel> keyImportance, List<string> missingKeys,
            ImportanceLevel level)
        {
            var keys = missingKeys.Where(key => keyImportance[key] == level).ToList();
            var missingKeyCount = keys.Count;
            if (missingKeyCount <= 0) return string.Empty;
            var listOfKeys = string.Join(", ", keys);
            return $"{listOfKeys}\n{missingKeyCount}";
        }
    }
}