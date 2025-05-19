using Neme.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using Neme.Services;

namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for SetRemindersDialog.xaml
    /// </summary>
    public partial class SetRemindersDialog : UserControl
    {
        public event Action ReminderSaved;

        public SetRemindersDialog()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate == null || string.IsNullOrWhiteSpace(TimeBox.Text))
            {
                MessageBox.Show("Please select date and time.");
                return;
            }

            if (!TimeSpan.TryParse(ReminderTime.SelectedTime.ToString(), out TimeSpan time))
            {
                MessageBox.Show("Invalid time format.");
                return;
            }

            var reminder = new Reminder
            {
                Title = TitleBox.Text,
                Notes = NotesBox.Text,
                ReminderDate = DatePicker.SelectedDate.Value.Date + time,
                IsCompleted = false
            };

            ReminderService.AddReminder(reminder);
            ReminderSaved?.Invoke(); // Notify parent window
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close(); // Or remove from dialog container
        }
    }
}
