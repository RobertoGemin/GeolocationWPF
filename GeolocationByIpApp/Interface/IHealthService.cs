using GeolocationApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Interface
{
    public interface IHealthService
    {
        HealthStatesModel HealthStatesModel { get; }
        event PropertyChangedEventHandler PropertyChanged;
        void UpdateHealthState(HealthServiceType healthServiceType, ResponseState responseState);
    }

}
