using System.ComponentModel;

namespace cleddmessenger.Models
{
    public class Peer : INotifyPropertyChanged
    {
        private PeerStatus _status;

        public string Name { get; set; } // Friendly name
        public string Address { get; set; } // Network address
        public string Avatar { get; set; } // Optional avatar path
        public List<string> Groups { get; set; } = new List<string>();

        public PeerStatus Status
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PeerStatus
    {
        Online,
        Offline,
        Unresponsive
    }
}
