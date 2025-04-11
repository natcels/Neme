using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Neme.Models
{
    public class AppTask : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _taskId;
        private AppTaskStatus _status;
        private string _taskName;
        private DateTime _startDate;
        private DateTime _endDate;

        public string TaskId
        {
            get => _taskId;
            set
            {
                _taskId = value;
                OnPropertyChanged(nameof(TaskId));
            }
        }

        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        public List<Peer> AssignedTo { get; set; } = new List<Peer>();

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public AppTaskStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        // Constructor to auto-generate TaskId
        public AppTask()
        {
            TaskId = GenerateTaskId();
        }

        private string GenerateTaskId()
        {
            return Guid.NewGuid().ToString();
        }
    }

    public enum AppTaskStatus
    {
        Initiated,
        InProgress,
        Completed,
        Overdue
    }
}
