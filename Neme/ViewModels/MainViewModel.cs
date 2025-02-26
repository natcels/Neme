using Neme.Models;
using Neme.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Neme.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Neme.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ChatViewModel ChatVM { get; }
        public ObservableCollection<Peer> Peers { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }

        private Peer _selectedPeer;
        public Peer SelectedPeer
        {
            get => _selectedPeer;
            set
            {
                _selectedPeer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPeerSelected));
              
                FilterMessages();
            }
        }

        public bool IsPeerSelected => SelectedPeer != null;



        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
                FilterMessages();
            }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand UploadFileCommand { get; }

        public ObservableCollection<ChatMessage> Messages { get; private set; }

      

        PeerDiscovery peerDiscovery;

        public MainViewModel()
        {
            ChatVM = new ChatViewModel();
            Messages = new ObservableCollection<ChatMessage>();
            Peers = new ObservableCollection<Peer>();
            Groups = new ObservableCollection<Group>();
           

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            UploadFileCommand = new RelayCommand(UploadFile);
            LoadPeers();
        }

        private void LoadPeers()
        {
            Peers.Clear();
            peerDiscovery = new PeerDiscovery("255.255.255.255", 5000, Environment.MachineName);
            List<string> discoveredPeerNames = peerDiscovery.GetDiscoveredPeers();

            foreach (var peerName in discoveredPeerNames)
            {
                Peers.Add(new Peer { Name = peerName, Address = "Unknown" });
            }
           
            if (!Peers.Any(p => p.Name == "Self"))
            {
                Peers.Add(new Peer { Name = "Self", Address = "127.0.0.1", Avatar="user-avatar.jpg"});
            }


        }

        private void SendMessage()
        {
            if (SelectedGroup != null)
                ChatVM.SendMessage(ChatVM.Messages.LastOrDefault()?.EncryptedContent, null, SelectedGroup);
            else if (SelectedPeer != null)
                ChatVM.SendMessage(ChatVM.Messages.LastOrDefault()?.EncryptedContent, SelectedPeer);
        }

        private void UploadFile()
        {
            if (SelectedPeer != null)
                ChatVM.UploadFile(SelectedPeer);
            else if (SelectedGroup != null)
                foreach (var peer in SelectedGroup.Members)
                    ChatVM.UploadFile(peer);
        }

        public void FilterMessages()
        {
            Messages.Clear();
            foreach (var msg in ChatVM.Messages.Where(m =>
                (SelectedPeer != null && m.ReceiverName == SelectedPeer.Name) ||
                (SelectedGroup != null && SelectedGroup.Members.Any(p => p.Name == m.ReceiverName))))
            {
                Messages.Add(msg);
            }
        }

        private bool CanSendMessage() => ChatVM.Messages.Any();
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
