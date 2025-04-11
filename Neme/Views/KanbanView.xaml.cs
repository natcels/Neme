using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Neme.Models;
using Neme.ViewModels;

namespace Neme.Views
{
    public partial class KanbanView : UserControl
    {
        private AppTask _draggedTask;

        public KanbanView()
        {
            InitializeComponent();
            DataContext = new KanbanViewModel();
            _draggedTask = new AppTask();
        }

        private void TaskList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is ListBox listBox)
            {
                _draggedTask = listBox.SelectedItem as AppTask;
                if (_draggedTask != null)
                {
                    DragDrop.DoDragDrop(listBox, _draggedTask, DragDropEffects.Move);
                }
            }
        }

        private void TaskList_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void TaskList_Drop(object sender, DragEventArgs e)
        {
            if (_draggedTask == null) return;

            if (sender is ListBox targetListBox)
            {
                var viewModel = DataContext as KanbanViewModel;

                if (viewModel != null)
                {
                    // Remove from the current list
                    viewModel.ToDoTasks.Remove(_draggedTask);
                    viewModel.InProgressTasks.Remove(_draggedTask);
                    viewModel.CompletedTasks.Remove(_draggedTask);

                    // Add to the new list based on column
                    if (targetListBox.Name == "ToDoList")
                    {
                        _draggedTask.Status = AppTaskStatus.Initiated;
                        viewModel.ToDoTasks.Add(_draggedTask);
                    }
                    else if (targetListBox.Name == "InProgressList")
                    {
                        _draggedTask.Status = AppTaskStatus.InProgress;
                        viewModel.InProgressTasks.Add(_draggedTask);
                    }
                    else if (targetListBox.Name == "CompletedList")
                    {
                        _draggedTask.Status = AppTaskStatus.Completed;
                        viewModel.CompletedTasks.Add(_draggedTask);
                    }
                }
            }

            _draggedTask = null;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            DialogOverlay.Visibility = Visibility.Visible;
            DialogOverlay.IsHitTestVisible = true;

            var fadeIn = (Storyboard)Resources["FadeInDialog"];
            fadeIn.Begin();

            AddTaskDialogControl.TaskSubmitted += OnTaskSubmitted;
        }

        private void OnTaskSubmitted(object? sender, AppTask newTask)
        {
            if (DataContext is KanbanViewModel viewModel)
            {
                switch (newTask.Status)
                {
                    case AppTaskStatus.Initiated:
                        viewModel.ToDoTasks.Add(newTask);
                        break;
                    case AppTaskStatus.InProgress:
                        viewModel.InProgressTasks.Add(newTask);
                        break;
                    case AppTaskStatus.Completed:
                        viewModel.CompletedTasks.Add(newTask);
                        break;
                }
            }
            DialogOverlay.Visibility = Visibility.Collapsed;
            DialogOverlay.IsHitTestVisible = false;
            AddTaskDialogControl.TaskSubmitted -= OnTaskSubmitted;
        }

      
    }
}
