using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Model
{
    public class NotificationModel
    {
        private NotificationModel(ResponseState state, string message)
        {
            ResponseState = state;
            Message = message;
        }

        public ResponseState ResponseState { get; private set; }
        public string Message { get; private set; }


        public static NotificationModel Success(string message = "")
        {
            return new NotificationModel(ResponseState.Success, message);
        }

        public static NotificationModel Notification(string message)
        {
            return new NotificationModel(ResponseState.Notification, message);
        }

        public static NotificationModel Fail(string message)
        {
            return new NotificationModel(ResponseState.Fail, message);
        }
    }
}