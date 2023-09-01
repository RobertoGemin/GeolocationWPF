using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeolocationApp.Model;
using GeolocationApp.Interface;

namespace GeolocationApp.Service
{
    public class NotificationService : INotificationService, INotifyPropertyChanged
    {
        private static readonly object NotificationLock = new object();
        private static readonly List<NotificationModel> NotificationsList = new List<NotificationModel>();

        public IReadOnlyList<NotificationModel> Notifications
        {
            get
            {
                lock (NotificationLock)
                {
                    return NotificationsList.ToList();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Success(string message)
        {
            AddNotification(NotificationModel.Success(message));
        }

        public void Notification(string message)
        {
            AddNotification(NotificationModel.Notification(message));
        }

        public void Fail(string message)
        {
            AddNotification(NotificationModel.Fail(message));
        }

        public void ClearNotifications()
        {
            lock (NotificationLock)
            {
                NotificationsList.Clear();
                NotifyPropertyChanged(nameof(Notifications));
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddNotification(NotificationModel notification)
        {
            lock (NotificationLock)
            {
                if (NotificationsList.LastOrDefault()?.Message.Equals(notification.Message) == true)
                {
                    return;
                }
                NotificationsList.Add(notification);
                NotifyPropertyChanged(nameof(Notifications));
            }
        }
    }
}