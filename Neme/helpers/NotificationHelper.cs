using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Neme.helpers
{
    public static class NotificationHelper
    {
        public static void ShowReminderToast(string title, string message, DateTime scheduledTime)
        {
            var content = new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .GetToastContent();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content.GetContent());

            var toast = new ScheduledToastNotification(xmlDoc, new DateTimeOffset(scheduledTime))
            {
                Id = Guid.NewGuid().ToString()
            };

            ToastNotificationManager.CreateToastNotifier("NemeApp").AddToSchedule(toast);
        }
    }
}
