using System;
using GeolocationApp.Model;
using GeolocationApp.Interface;
using GeolocationApp.Service;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace GeolocationConsoleApp
{
    internal class Program
    {
        public static IHealthService HealthService;
        public static ILoggingService LoggingService;
        public static INotificationService NotificationService;
        public static IConfigSettingService ConfigSettingService;
        public static IDatabaseService SqlLiteService;
        public static IApiService ApiService;
        private static async Task Main(string[] args)
        {
            HealthService = new HealthService();
            LoggingService = new LoggingService(HealthService, "console_user");
            NotificationService = new NotificationService();
            NotificationService.PropertyChanged += NotificationService_PropertyChanged;
            ConfigSettingService = new ConfigSettingService(NotificationService, LoggingService, HealthService);
            SqlLiteService = new SqlLiteService(NotificationService, LoggingService, HealthService);
            ApiService = new ApiIpifyService(NotificationService, LoggingService, HealthService);
            await Start();
            Console.ReadLine();

        }


        public static async Task Start()
        {
            await ValidateService();
            await CreateTables();
            await AddInfo();

        }

        public static async Task ValidateService()
        {
            if (ConfigSettingService.CheckAppConfigKeys())
                NotificationService.Success("ConfigSettingService is Validate");
            else
                NotificationService.Fail("ConfigSettingService is not valid.");


            if (await ApiService.IsApiValid())
                NotificationService.Success("API is Validate");
            else
                NotificationService.Fail("APi is not valid.");
        }
        
        public static async Task AddInfo()
        {
            LoggingService.Log("AddInfo");

            var ipAddressModels = new List<IpAdressModel>
            {
                new IpAdressModel
                {
                    Id = "142.250.72.174", City = "US", Region = "California", Country = "US", Latitude = "3 405 223",
                    Longitude = "-11 824 368"
                },
                new IpAdressModel
                {
                    Id = "142.250.217.132", City = "TW", Region = "Taiwan", Country = "TW", Latitude = "2 407 327",
                    Longitude = "12 056 276"
                },
                new IpAdressModel
                {
                    Id = "192.0.78.13", City = "US", Region = "California", Country = "US", Latitude = "3 777 493",
                    Longitude = "-12 241 942"
                },
                new IpAdressModel
                {
                    Id = "3.33.139.32", City = "US", Region = "Washington", Country = "US", Latitude = "4 760 621",
                    Longitude = "-12 233 207"
                },
                new IpAdressModel
                {
                    Id = "142.250.68.14", City = "US", Region = "California", Country = "US", Latitude = "3 405 223",
                    Longitude = "-11 824 368"
                },
                new IpAdressModel
                {
                    Id = "142.250.188.227", City = "US", Region = "California", Country = "US", Latitude = "3 405 223",
                    Longitude = "-11 824 368"
                },
                new IpAdressModel
                {
                    Id = "142.250.217.131", City = "TW", Region = "Taiwan", Country = "TW", Latitude = "2 407 327",
                    Longitude = "12 056 276"
                },
                new IpAdressModel
                {
                    Id = "173.223.234.136", City = "US", Region = "California", Country = "US", Latitude = "3 391 918",
                    Longitude = "-11 841 647"
                },
                new IpAdressModel
                {
                    Id = "52.96.10.82", City = "US", Region = "California", Country = "US", Latitude = "3 733 939",
                    Longitude = "-12 189 496"
                },
                new IpAdressModel
                {
                    Id = "52.96.222.226", City = "US", Region = "Texas", Country = "US", Latitude = "2 942 412",
                    Longitude = "-9 849 363"
                }
            };

            var domainModels = new List<DomainModel>
            {
                new DomainModel { Name = "www.google.com", IpAdressId = "142.250.217.132" },
                new DomainModel { Name = "www.wordpress.com", IpAdressId = "192.0.78.13" },
                new DomainModel { Name = "www.developer.wordpress.com", IpAdressId = "192.0.78.13" },
                new DomainModel { Name = "www.wordpress.tk", IpAdressId = "3.33.139.32" },
                new DomainModel { Name = "google.com", IpAdressId = "142.250.68.14" },
                new DomainModel { Name = "google.pl", IpAdressId = "142.250.188.227" },
                new DomainModel { Name = "google.nl", IpAdressId = "142.250.217.131" },
                new DomainModel { Name = "www.nu.nl", IpAdressId = "173.223.234.136" },
                new DomainModel { Name = "www.outlook.com", IpAdressId = "52.96.10.82" },
                new DomainModel { Name = "outlook.com", IpAdressId = "52.96.222.226" }
            };
            foreach (var ipAddressModel in ipAddressModels)
            {
                var matchingDomainModels =
                    domainModels.Where(domain => domain.IpAdressId == ipAddressModel.Id).ToList();

                if (matchingDomainModels.Count == 0)
                {
                    matchingDomainModels.Add(new DomainModel());
                }
                foreach (var domainModel in matchingDomainModels)
                {
                    if (await SqlLiteService.Insert(ipAddressModel, domainModel))
                    {
                        var matchingDomains = !string.IsNullOrEmpty(domainModel.Name)
                            ? $", Domain: {domainModel.Name}"
                            : string.Empty;
                        Console.WriteLine($"Inserting: IP: {ipAddressModel.Id}{matchingDomains} successful");
                    }

                }

            }

        }
        public static async Task CreateTables()
        {
            LoggingService.Log("CreateTables");


            if (await SqlLiteService.ValidateDatabase())
            {
                if (!await SqlLiteService.CheckTableExist(new IpAdressModel()))
                {
                    _ = await SqlLiteService.CreateTable(new IpAdressModel());

                }

                if (!await SqlLiteService.CheckTableExist(new DomainModel()))
                {
                    _ = await SqlLiteService.CreateTable(new DomainModel());

                }
            }
         

        }

        private static void NotificationService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotificationService.Notifications))
            {
                var notificationService = (NotificationService)sender;

                var lastNotification = notificationService.Notifications.LastOrDefault();
                if (lastNotification != null)
                {
                    Console.WriteLine($"Notification: {lastNotification.Message}");
                }
            }
        }


    }
}