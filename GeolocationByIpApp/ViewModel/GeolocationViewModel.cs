using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GeolocationApp.Model;
using GeolocationApp.Commands;
using GeolocationApp.Methods;
using GeolocationApp.Interface;
using static GeolocationApp.Model.Enums;
using GeolocationApp.Service;

namespace GeolocationApp.ViewModel
{
    public class GeolocationViewModel : INotifyPropertyChanged
    {
        private List<DomainModel> _domainModels;
        private IpDomainSearchModel _iDomainSearchModel;

        private IpAdressModel _ipadressModel;
        private string _searchTextBox = string.Empty;
        public IApiService ApiService;

        public IConfigSettingService ConfigSettingService;

        public IDatabaseService DatabaseService;
        public IInputValidationService InputValidationService;

        public bool IsLoadDataRunning;
        public List<string> ListNotInApi = new List<string>();
        public ILoggingService LoggingService;
        public INotificationService NotificationService;
        public IHealthService HealthService;



        private HealthStatesModel _healthStatesModel { get; set; }
        public HealthStatesModel HealthStatesModel
        {
            get => _healthStatesModel;
            set
            {
                _healthStatesModel = value;
                OnPropertyChanged(nameof(HealthStatesModelPropertiesFirst));
                OnPropertyChanged(nameof(HealthStatesModelPropertiesSecond));


            }
        }
        public IEnumerable<KeyValuePair<string, ResponseState>> HealthStatesModelPropertiesFirst
        {
            get
            {
                yield return new KeyValuePair<string, ResponseState>("ApiService", _healthStatesModel.ApiService);
                yield return new KeyValuePair<string, ResponseState>("InputService", _healthStatesModel.InputService);
                yield return new KeyValuePair<string, ResponseState>("SqlLiteService", _healthStatesModel.SqlLiteService);

            }
        }
        public IEnumerable<KeyValuePair<string, ResponseState>> HealthStatesModelPropertiesSecond
        {
            get
            {
                yield return new KeyValuePair<string, ResponseState>("LoggingService", _healthStatesModel.LoggingService);
                yield return new KeyValuePair<string, ResponseState>("ConfigService", _healthStatesModel.ConfigService);

            }
        }



        public IpAdressModel IpadressModel
        {
            get => _ipadressModel;
            set
            {
                _ipadressModel = value;
                OnPropertyChanged(nameof(IpadressModel));
            }
        }

        public List<DomainModel> DomainList
        {
            get => _domainModels;
            set
            {
                _domainModels = value;
                OnPropertyChanged(nameof(DomainList));
            }
        }

        public string SearchTextBox
        {
            get => _searchTextBox;
            set
            {
                _searchTextBox = value;
                OnPropertyChanged(nameof(SearchTextBox));
                if (HealthStatesModel.AreAllServicesSuccessful())
                {
                    IpDomainSearchModel = InputValidationService.GetIpDomainSearchModel(value);

                }

            }
        }

        public IpDomainSearchModel IpDomainSearchModel
        {
            get => _iDomainSearchModel;
            set
            {
                _iDomainSearchModel = value;
                if (value.EntryDataType != Enums.EntryType.None)
                {
                    GetModelsFromDb();
                }
                else
                {
                    IpadressModel = new IpAdressModel();
                    DomainList = new List<DomainModel>();
                }

                OnPropertyChanged(nameof(IpDomainSearchModel));
            }
        }

        public SearchCommand SearchCommand { get; set; }
        public DeleteCommand DeleteCommand { get; set; }


        public GeolocationViewModel(ILoggingService loggingService, INotificationService notificationService, IHealthService healthService,
            IInputValidationService inputValidationService,
            IConfigSettingService configSettingService,
            IApiService apiService,
            IDatabaseService databaseService
        )
        {
            LoggingService = loggingService;


            InputValidationService = inputValidationService;
            ConfigSettingService = configSettingService;
            NotificationService = notificationService;
            Notifications = new ObservableCollection<NotificationModel>(NotificationService.Notifications);
            HealthService = healthService;
            HealthStatesModel = healthService.HealthStatesModel;


            ApiService = apiService;
            DatabaseService = databaseService;
            IpadressModel = null;
            DomainList = null;

            IpDomainSearchModel = new IpDomainSearchModel();
            SearchCommand = new SearchCommand(this);
            DeleteCommand = new DeleteCommand(this);

            NotificationService.PropertyChanged += OnNotificationServicePropertyChanged;
            HealthService.PropertyChanged += HealthService_PropertyChanged;
        }

      


        public ObservableCollection<NotificationModel> Notifications { get; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnWindowLoaded()
        {
            IpDomainSearchModel = InputValidationService.GetIpDomainSearchModel(string.Empty);
            ValidateService();
        }

        public async void ValidateService()
        {
            if (ConfigSettingService.CheckAppConfigKeys())
            {
                NotificationService.Success("ConfigSettingService is Validate");
            }
            else
            {
                NotificationService.Fail("ConfigSettingService is not valid.");
            }

            if (await ValidateDatabase())
            {
                NotificationService.Success("Database is Validate");
            }
            else
            {
                NotificationService.Fail("Database is not valid.");
            }

            if (await ApiService.IsApiValid())
            {
                NotificationService.Success("API is Validate");
            }
            else
            {
                NotificationService.Fail("APi is not valid.");
            }
        }
    
        private async Task<bool> ValidateDatabase()
        {
            return await DatabaseService.ValidateDatabase() &&
                   await DatabaseService.CheckTableExist(new IpAdressModel()) &&
                   await DatabaseService.CheckTableExist(new DomainModel());
        }


        public async void GetModelsFromDb()
        {
            if (IsLoadDataRunning) return;

            IsLoadDataRunning = true;

            if (!HealthStatesModel.AreAllServicesSuccessful())
            {
                return;
            }

            if (ListNotInApi.Contains(IpDomainSearchModel.Id))
            {
                NotificationService.Notification(
                    $"Warning {IpDomainSearchModel.EntryDataType}: '{IpDomainSearchModel.Id}' does not exist.");
            }
            else
            {
                IpadressModel = await DatabaseService.GetIpAdress(IpDomainSearchModel);
                if (!string.IsNullOrEmpty(IpadressModel.Id))
                    DomainList = await DatabaseService.GetDomainList(IpDomainSearchModel);
                else
                    DomainList = new List<DomainModel>();
            }

            IsLoadDataRunning = false;
        }


        public async void DeleteModels()
        {
            if (await DatabaseService.Delete(IpDomainSearchModel))
            {
                NotificationService.Success(
                    $"Success: {IpDomainSearchModel.EntryDataType} with ID '{IpDomainSearchModel.Id}' successfully deleted from the database.");
            }
            else
            {   NotificationService.Notification(
                    $"Error: Unable to delete {IpDomainSearchModel.EntryDataType} with '{IpDomainSearchModel.Id}' from the database.");
            }SearchTextBox = string.Empty;
        }

        public async void GetModelsFromApi()
        {
            var apiModel = await ApiService.GetData(IpDomainSearchModel);

            if (!string.IsNullOrEmpty(apiModel.ip))
            {
                ApiModelToDbModel.ConvertModel(apiModel, out var ipadressModel, out var domainModel);
                if (await DatabaseService.Insert(ipadressModel, domainModel))
                    NotificationService.Success(
                        $"{IpDomainSearchModel.EntryDataType}: '{IpDomainSearchModel.Id}' successfully retrieved on the webserver and successfully inserted in the database.");
                else
                    NotificationService.Notification(
                        $"{IpDomainSearchModel.EntryDataType}: '{IpDomainSearchModel.Id}' successfully retrieved on the webserver and  unsuccessfully inserted in the database.");
                GetModelsFromDb();
            }
            else
            {
                NotificationService.Notification(
                    $"The {IpDomainSearchModel.EntryDataType} with '{IpDomainSearchModel.Id}' does not exist on the webserver.");
                ListNotInApi.Add(IpDomainSearchModel.Id);
                SearchTextBox = string.Empty;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void HealthService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e.PropertyName == "HealthStatesModel")
                    {
                        HealthStatesModel = HealthService.HealthStatesModel;

                        OnPropertyChanged(nameof(HealthStatesModel)); 
                    }
                });
            });
         
        }

        private void OnNotificationServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Task.Run(() =>
            {
                var notifications = NotificationService.Notifications.Reverse().ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifications.Clear();
                    foreach (var notification in notifications) Notifications.Add(notification);
                });
            });
        }
    }
}