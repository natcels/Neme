using Neme.Models;
using Neme.Utils;
using System.Configuration;
using System.Data;
using System.Windows;
using CommunityToolkit.WinUI.Notifications;

namespace Neme
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ToastSupport.RegisterAppForToasts();

            var notifier = ToastNotificationManager.CreateToastNotifier("NemeApp");
            foreach (var toast in notifier.GetScheduledToastNotifications())
            {
                if (toast.DeliveryTime < DateTimeOffset.Now)
                    notifier.RemoveFromSchedule(toast);
            }
            base.OnStartup(e);
        }


    }

}
