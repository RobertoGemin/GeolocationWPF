using GeolocationApp.Interface;
using GeolocationApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Service
{
    public class HealthService: IHealthService
    {
        private static readonly object HealthLock = new object();
        private static readonly HealthStatesModel HealthStates = new HealthStatesModel();

        public HealthStatesModel HealthStatesModel 
        {
            get
            {
                lock (HealthLock)
                {
                    return HealthStates;
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateHealthState(HealthServiceType healthServiceType, ResponseState responseState)
        {
            lock (HealthLock)
            {
                switch (healthServiceType)
                {
                    case HealthServiceType.ApiService:
                        HealthStates.ApiService = responseState;
                        break;
                    case HealthServiceType.ConfigService:
                        HealthStates.ConfigService = responseState;
                        break;
                    case HealthServiceType.InputService:
                        HealthStates.InputService = responseState;
                        break;
                    case HealthServiceType.LoggingService:
                        HealthStates.LoggingService = responseState;
                        break;
                    case HealthServiceType.SqlLiteService:
                        HealthStates.SqlLiteService = responseState;
                        break;
                    default:
                    
                        return;
                }

                NotifyPropertyChanged(nameof(HealthStatesModel));
            }
        }
    }
}
