using System.Windows;
using GeolocationApp.Interface;
using GeolocationApp.Service;
using GeolocationApp.Service;

namespace GeolocationApp
{
    public partial class App : Application
    {
        public App()
        {
            DependencyInjector.Inject<INotificationService>(new NotificationService());
            DependencyInjector.Inject<IHealthService>(new HealthService());


            var healthService = DependencyInjector.Retrieve<IHealthService>();
            DependencyInjector.Inject<ILoggingService>(new LoggingService(healthService));


            var notificationService = DependencyInjector.Retrieve<INotificationService>();
            var loggingService = DependencyInjector.Retrieve<ILoggingService>();

            DependencyInjector.Inject<IApiService>(new ApiIpifyService(
                notificationService, loggingService, healthService));
            DependencyInjector.Inject<IInputValidationService>(new InputValidationService(
                notificationService, loggingService,healthService));
            DependencyInjector.Inject<IConfigSettingService>(new ConfigSettingService(
                notificationService, loggingService, healthService));
            DependencyInjector.Inject<IDatabaseService>(new SqlLiteService(
                notificationService, loggingService, healthService));


            MainWindow = DependencyInjector.Retrieve<MainWindow>();
            MainWindow.Show();
        }
    }
}