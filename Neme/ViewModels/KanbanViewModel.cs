using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neme.Models;

namespace Neme.ViewModels
{
    public class KanbanViewModel : BaseViewModel
    {
        public ObservableCollection<AppTask> ToDoTasks { get; set; }
        public ObservableCollection<AppTask> InProgressTasks { get; set; }
        public ObservableCollection<AppTask> CompletedTasks { get; set; }

        public ICommand AddTaskCommand { get; }

        public KanbanViewModel()
        {
            ToDoTasks = new ObservableCollection<AppTask>();
            InProgressTasks = new ObservableCollection<AppTask>();
            CompletedTasks = new ObservableCollection<AppTask>();

            AddTaskCommand = new RelayCommand(AddTask);

            // Sample Data for Testing
            ToDoTasks.Add(new AppTask { TaskName = "Fix UI Bug", Status = AppTaskStatus.Initiated });
            InProgressTasks.Add(new AppTask { TaskName = "Implement Login", Status = AppTaskStatus.InProgress });
            CompletedTasks.Add(new AppTask { TaskName = "Database Migration", Status = AppTaskStatus.Completed });
        }

        public void AddTask(object parameter)
        {
            ToDoTasks.Add(new AppTask { TaskName = "New Task", Status = AppTaskStatus.Initiated });
        }
    }
}
