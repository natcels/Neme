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
using Neme.Models;


namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for AddTaskDialog.xaml
    /// </summary>
    public partial class AddTaskDialog : UserControl
    {
        public event EventHandler<AppTask> TaskSubmitted;
        public event EventHandler<AppTask> TaskCanceled;

        public AddTaskDialog()
        {
            InitializeComponent();
        }


        private void SubmitTask_Click(object sender, RoutedEventArgs e)
        {
            string name = TaskNameBox.Text;
            var selectedItem = (StatusBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrWhiteSpace(name) && selectedItem != null)
            {
                var task = new AppTask
                {
                    TaskName = name,
                    Status = selectedItem switch
                    {
                        "Initiated" => AppTaskStatus.Initiated,
                        "In Progress" => AppTaskStatus.InProgress,
                        "Completed" => AppTaskStatus.Completed,
                        _ => AppTaskStatus.Initiated
                    },
                    StartDate =(DateTime) StartDatePicker.SelectedDate?.Date,
                    EndDate =(DateTime) EndDatePicker.SelectedDate?.Date
                };

                TaskSubmitted?.Invoke(this, task);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Panel parentPanel)
            {
                parentPanel.Visibility = Visibility.Collapsed;
                parentPanel.IsHitTestVisible = false;

            }
        }
    }
}

