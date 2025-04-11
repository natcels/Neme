using System;
using System.Collections.ObjectModel;
using Neme.Models;

namespace Neme.ViewModels
{
    public class PeerViewModel : BaseViewModel
    {
        public Peer Peer { get; }

        public PeerViewModel(Peer peer)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
        }

        public string Name => Peer.Name;
        public string Address => Peer.Address;
        public string Avatar => Peer.Avatar;
        public PeerStatus Status
        {
            get => Peer.Status;
            set
            {
                Peer.Status = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Groups { get; set; } = new ObservableCollection<string>();

        public string PublicKey => Peer.PublicKey;

        /// <summary>
        /// Updates the peer's status and last seen time.
        /// </summary>
        public void UpdateStatus(PeerStatus newStatus)
        {
            Status = newStatus;
        }

        /// <summary>
        /// Marks the peer as offline.
        /// </summary>
        public void MarkOffline()
        {
            Status = PeerStatus.Offline;
        }
    }
}
