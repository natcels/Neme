using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Neme.Models
{
    public class Group :  INotifyPropertyChanged
    {
        private string groupName;
        public string GroupName 
        {
                get => groupName;
                set
            {
                    groupName = value;
                    OnPropertyChanged();
                }
            }
        public ObservableCollection<Peer> Members { get; set; } = new ObservableCollection<Peer>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
