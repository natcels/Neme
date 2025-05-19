using Neme.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : UserControl
    {
        public CalendarView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SetRemindersDialog();
            dialog.ReminderSaved += LoadUpcomingReminders;

            var dialogHost = new Window
            {
                Content = dialog,
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                //Owner = this
            };
            dialogHost.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void LoadUpcomingReminders()
        {
            var reminders = ReminderService.GetAllReminders()
                                           .Where(r => r.ReminderDate>= DateTime.Now && !r.IsCompleted)
                                           .OrderBy(r => r.ReminderDate)
                                           .ToList();

            UpcomingRemindersList.ItemsSource = reminders;
        }
    }
}
