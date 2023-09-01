using System.Collections.Generic;
using System.ComponentModel;
using GeolocationApp.Model;

namespace GeolocationApp.Interface
{
    public interface INotificationService
    {
        IReadOnlyList<NotificationModel> Notifications { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void Success(string message);
        void Notification(string message);
        void Fail(string message);
        void ClearNotifications();
    }
}