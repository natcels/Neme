using System;
using System.Collections.Generic;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Windows;

namespace Neme.Helpers
{
    public class NotificationService
    {
        private readonly Notifier _notifier;

        public NotificationService()
        {
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    Application.Current.MainWindow,
                    Corner.BottomRight,
                    10, 10);
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(3),
                    MaximumNotificationCount.FromCount(5));
            });
        }

        public void ShowMessage(string message)
        {
            //_notifier.ShowInformation(message);
        }
    }
}
