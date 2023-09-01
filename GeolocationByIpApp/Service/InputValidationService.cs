using System;
using GeolocationApp.Interface;
using GeolocationApp.Model;
using GeolocationApp.Interface;
using GeolocationApp.Methods;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Service
{
    public class InputValidationService : IInputValidationService
    {
        public ILoggingService LoggingService;
        public INotificationService NotificationService;
        public IHealthService HealthService;



        public InputValidationService(INotificationService notificationService, ILoggingService loggingService, IHealthService healthService)
        {
            NotificationService = notificationService;
            LoggingService = loggingService;
            HealthService = healthService;
        }

        public IpDomainSearchModel GetIpDomainSearchModel(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var getIpDomain = input.Length > 3
                        ? Validate.ValidateText(input)
                        : IpDomainSearchModel.Empty();
                    if (getIpDomain.EntryDataType != EntryType.None)
                    {
                        NotificationService.Success($"Search for {getIpDomain.EntryDataType}: {getIpDomain.Id}");

                        return getIpDomain;
                    }
                }

                NotificationService.Notification("Insert Domain or IP adress");
                return IpDomainSearchModel.Empty();
            }
            catch (Exception ex)
            {
                NotificationService.Fail("Search function isn't functioning correctly. Please contact support");
                LoggingService.Log($"Exception: {ex.Message} \n");
                HealthService.UpdateHealthState(HealthServiceType.InputService, ResponseState.Fail);

                return IpDomainSearchModel.Empty();
            }
        }
    }
}