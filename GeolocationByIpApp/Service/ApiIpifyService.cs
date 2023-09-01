using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GeolocationApp.Model;
using GeolocationApp.Interface;
using GeolocationApp.Methods;
using Newtonsoft.Json;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Service
{
    public class ApiIpifyService : IApiService
    {
        public ILoggingService LoggingService;
        public INotificationService NotificationService;
        public IHealthService HealthService;

        public ApiIpifyService(INotificationService notificationService, ILoggingService loggingService, IHealthService healthService)
        {
            NotificationService = notificationService;
            LoggingService = loggingService;
            HealthService = healthService;
        }


        public async Task<bool> IsApiValid()
        {
            var apiKey = ConfigurationManager.AppSettings["ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                NotificationService.Fail(
                    "The application is missing an API key. Please contact support for assistance.");

                HealthService.UpdateHealthState(HealthServiceType.ApiService, ResponseState.Fail);

                return false;
            }

            if (!await TestEndpointAvailability())
            {
                NotificationService.Fail("A network error has occurred.Please try again later or contact support.");
                HealthService.UpdateHealthState(HealthServiceType.ApiService, ResponseState.Fail);

                return false;
            }

            var errorMessage = await TestApiBalance(apiKey);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                HealthService.UpdateHealthState(HealthServiceType.ApiService, ResponseState.Fail);
                return false;
            }


            return true;
        }


        public async Task<APIIpifyModel> GetData(IpDomainSearchModel myApiIParameter)
        {
            APIIpifyModel ipInfoApiModel;
            var apiKey = ConfigurationManager.AppSettings["ApiKey"];
            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            var url = string.Empty;
            try
            {
                if (myApiIParameter.EntryDataType == EntryType.DomainName)
                    url = apiUrl + $"apiKey={apiKey}&domain={myApiIParameter.Id}";
                else if (myApiIParameter.EntryDataType == EntryType.IPAddress)
                    url = apiUrl + $"apiKey={apiKey}&ipAddress={myApiIParameter.Id}";
                else
                    return new APIIpifyModel();


                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync($"{url}");

                    if (result.IsSuccessStatusCode)
                    {
                        var jsonResult = await result.Content.ReadAsStringAsync();
                        ipInfoApiModel = JsonConvert.DeserializeObject<APIIpifyModel>(jsonResult);

                    
                            if (ipInfoApiModel.location.country == "ZZ" ||
                                string.IsNullOrEmpty(ipInfoApiModel.location.region))
                            {
                                NotificationService.Notification(
                                    $"{myApiIParameter.EntryDataType} with ID \"{myApiIParameter.Id}\" does not exist.");
                                return new APIIpifyModel();
                            }

                            if (myApiIParameter.EntryDataType == EntryType.DomainName)
                                ipInfoApiModel.domainName = myApiIParameter.Id;
                            NotificationService.Success(
                                $"{myApiIParameter.EntryDataType.ToString()} \"{myApiIParameter.Id}\" retrieved");


                            return ipInfoApiModel;
                        

                        var apiError = JsonConvert.DeserializeObject<APIErrorMessageModel>(jsonResult);
                        NotificationService.Notification(
                            $"{apiError.code} \n {apiError.messages}");
                        return new APIIpifyModel();
                    }

                    if (myApiIParameter.EntryDataType == EntryType.DomainName &&
                        result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        NotificationService.Notification(
                            $"{myApiIParameter.EntryDataType} with ID \"{myApiIParameter.Id}\" does not exist.");

                        return new APIIpifyModel();
                    }

                    NotificationService.Fail($"{Validate.GetResponseStatusCodes(result.StatusCode)}");
                    LoggingService.Log($"{Validate.GetResponseStatusCodes(result.StatusCode)}");
                    return new APIIpifyModel();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Fail("A network error has occurred.Please try again later or contact support.");
                LoggingService.Log($"Exception: {ex.Message} \n");
                return new APIIpifyModel();
            }
        }

        private async Task<string> TestApiBalance(string apiKey)
        {
            var urlBalance = ConfigurationManager.AppSettings["ApiUrlBalance"];
            var apiUrlBalance = urlBalance.Replace("{apiKey}", apiKey);

            var message = string.Empty;
            var log = string.Empty;

            var balance = new ApiBalanceModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync($"{apiUrlBalance}");

                    if (result.IsSuccessStatusCode)
                    {
                        var jsonResult = await result.Content.ReadAsStringAsync();
                        balance = JsonConvert.DeserializeObject<ApiBalanceModel>(jsonResult);
                        if (string.IsNullOrEmpty(balance.error) && balance.credits > 0)
                        {

                            var defaultThreshold = 100;
                            int apiCallThreshold;
                            var apiCallThresholdValue = ConfigurationManager.AppSettings.Get("ApiCallThreshold");
                            if (string.IsNullOrEmpty(apiCallThresholdValue) ||
                                !int.TryParse(apiCallThresholdValue, out apiCallThreshold))
                            {
                                apiCallThreshold = defaultThreshold;
                            }

                            if (balance.credits >= apiCallThreshold)
                            {
                                NotificationService.Success( "The API currently maintains a balanced amount of resources.");
                            }
                            else
                            {
                                NotificationService.Notification($"The API currently maintains a balanced amount of resources. Warning: Only {balance.credits} API calls left.");
                            }
                            return string.Empty;
                        }
                        else
                        {
                            NotificationService.Notification($"The API currently maintains a balanced amount of resources. Warning: Only {balance.credits} API calls left.");
                            message = GetBalancedFailureMessage();
                            return message;
                        }
                    }

                    message = GetAPIcallFailureMessage();
                    NotificationService.Fail("The API is experiencing an imbalance in its resources.");

                    return message;
                }
            }
            catch (Exception ex)
            {
                message = "Failed to complete the HTTP request. Please check your network connection and try again.";
                NotificationService.Fail(
                    "Failed to complete the HTTP request. Please check your network connection and try again.");
                LoggingService.Log($"Exception: {ex.Message} \n");
                HealthService.UpdateHealthState(HealthServiceType.ApiService, ResponseState.Fail);

                return message;
            }
        }

        private async Task<bool> TestEndpointAvailability()
        {
            var endpointUrl = ConfigurationManager.AppSettings["EndpointUrl"];

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(endpointUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        NotificationService.Success("HTTP request is operational");
                    }
                    else
                    {
                        NotificationService.Fail($"Error code:{response.StatusCode}. Please contact support");
                        LoggingService.Log($"{Validate.GetResponseStatusCodes(response.StatusCode)} \n");
                    }

                    return response.IsSuccessStatusCode;
                }
                catch (HttpRequestException ex)
                {
                    NotificationService.Fail("A network error has occurred.Please try again later or contact support.");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    return false;
                    HealthService.UpdateHealthState(HealthServiceType.ApiService, ResponseState.Fail);

                }
            }
        }

        #region internal class

        internal class ApiBalanceModel
        {
            public int credits { get; set; }
            public string error { get; set; }
        }

        internal class APIErrorMessageModel
        {
            public int code { get; set; }
            public string messages { get; set; }
        }

        #endregion

        #region message

        private static string GetBalancedFailureMessage()
        {
            return "The API is experiencing an imbalance in its resources.";
        }

        private static string GetBalancedSuccesMessage(int credits)
        {
            var defaultThreshold = 100;
            int apiCallThreshold;
            var apiCallThresholdValue = ConfigurationManager.AppSettings.Get("ApiCallThreshold");
            if (string.IsNullOrEmpty(apiCallThresholdValue) ||
                !int.TryParse(apiCallThresholdValue, out apiCallThreshold)) apiCallThreshold = defaultThreshold;

            if (credits >= apiCallThreshold)
                return "The API currently maintains a balanced amount of resources.";
            return
                $"The API currently maintains a balanced amount of resources. Warning: Only {credits} API calls left.";
        }

        private static string GetAPIcallFailureMessage()
        {
            return "The API call was not successful.";
        }

        private static string GetHttpRequestFailureMessage()
        {
            return "Failed to complete the HTTP request. Please check your network connection and try again.";
        }

        #endregion
    }
}