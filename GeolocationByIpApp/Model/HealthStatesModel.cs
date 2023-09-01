using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Model
{
    public class HealthStatesModel
    {
        public ResponseState ApiService { get; set; }
        public ResponseState ConfigService { get; set; }
        public ResponseState InputService { get; set; }
        public ResponseState LoggingService { get; set; }
        public ResponseState SqlLiteService { get; set; }


       public HealthStatesModel()
       {
           ApiService = ResponseState.Success;
           ConfigService = ResponseState.Success;
           InputService = ResponseState.Success;
           LoggingService = ResponseState.Success;
           SqlLiteService = ResponseState.Success;
       }
       public bool AreAllServicesSuccessful()
       {
           return ApiService == ResponseState.Success &&
                  ConfigService == ResponseState.Success &&
                  InputService == ResponseState.Success &&
                  LoggingService == ResponseState.Success &&
                  SqlLiteService == ResponseState.Success;
       }

    }
}
