using Neme.Utils;
using Neme.Models;
using PeerStatus = Neme.Models.PeerStatus;
using Neme.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Windows;


namespace Neme.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Peer> Peers { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<ChatMessage> Messages { get; private set; }

        private Peer _selectedPeer;
        public Peer SelectedPeer
        {
            get => _selectedPeer;
            set
            {
                _selectedPeer = value;
                OnPropertyChanged();
                FilterMessages();
            }
        }

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

        private string _currentMessage;
        public string CurrentMessage
        {
            get => _currentMessage;
            set
            {
                _currentMessage = value;
                OnPropertyChanged();
                ((RelayCommand)SendMessageCommand).RaiseCanExecuteChanged();
            }
        }

        private readonly NotificationService _notificationService;
        private readonly System.Timers.Timer _heartbeatTimer;
        private readonly string _dbConnectionString = "Data Source=messages.db;Version=3;";
        private readonly GroupManager _groupManager;
        private AesEncryption _aesEncryption;

        public ICommand SendMessageCommand { get; }
        public ICommand UploadFileCommand { get; }
        public ICommand CreateGroupCommand { get; }
        public ICommand RenameGroupCommand { get; }
        public ICommand AddPeerToGroupCommand { get; }

        public MainViewModel()
        {
            _notificationService = new NotificationService();
            _groupManager = new GroupManager();
            Peers = new ObservableCollection<Peer>();
            Groups = new ObservableCollection<Group>();
            Messages = new ObservableCollection<ChatMessage>();

            _aesEncryption = new AesEncryption();

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            UploadFileCommand = new RelayCommand(UploadFile);
            CreateGroupCommand = new RelayCommand(CreateGroup);
            RenameGroupCommand = new RelayCommand(RenameGroup);
            AddPeerToGroupCommand = new RelayCommand<Peer>(AddPeerToGroup);

            LoadPeers();
            LoadGroups();
            LoadMessagesFromDatabase();

            _heartbeatTimer = new System.Timers.Timer(5000);
            _heartbeatTimer.Elapsed += SendHeartbeat;
            _heartbeatTimer.Start();
        }

        private PeerDiscovery peerDiscovery;

        private void LoadPeers()
        {
            Peers.Clear(); // Clear the list before refreshing

             peerDiscovery = new PeerDiscovery("255.255.255.255", 5000, Environment.MachineName);
            List<string> discoveredPeerNames = peerDiscovery.GetDiscoveredPeers();

            foreach (var peerName in discoveredPeerNames)
            {
                Peers.Add(new Peer { Name = peerName, Address = "Unknown" }); // IP can be updated later
            }

            // Ensure the "Self" peer is always present
            if (!Peers.Any(p => p.Name == "Self"))
            {
                Peers.Add(new Peer { Name = "Self", Address = "127.0.0.1" });
            }
        }



        private void LoadGroups()
        {
            var savedGroups = _groupManager.GetGroups();
            foreach (var group in savedGroups)
            {
                Groups.Add(new Group { GroupName = group.GroupName });
            }
        }

        private void FilterMessages()
        {
            Messages.Clear();
            foreach (var msg in Messages.Where(m =>
                (SelectedPeer != null && m.SenderName == SelectedPeer.Name)))
            {
                Messages.Add(msg);
            }
        }

        private void FilterMessagesByGroup()
        {
            Messages.Clear();
            foreach (var msg in Messages.Where(m =>
               (SelectedGroup != null && SelectedGroup.Members.Any(p => p.Name == m.SenderName))))
                {
                Messages.Add(msg);
            }
            }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessage)) return;

            if (SelectedGroup != null)
            {
                foreach (var peer in SelectedGroup.Members)
                {
                    SendMessageToPeer(CurrentMessage, peer);
                }
            }
            else if (SelectedPeer != null)
            {
                SendMessageToPeer(CurrentMessage, SelectedPeer);
            }

            Messages.Add(new ChatMessage
            {
                SenderName = "You",
                Content = CurrentMessage,
                Timestamp = DateTime.Now,
                IsSentByCurrentUser = true,
                ReceiverName = SelectedPeer.Name
            });

//            SaveMessagesToDatabase();
  //          CurrentMessage = string.Empty;

        }

        private void SendMessageToPeer(string message, Peer peer)
        {
        
            //Messages.Add(new ChatMessage
            //{
            //    Content = _aesEncryption.Encrypt(CurrentMessage),
            //    ReceiverName = SelectedPeer.Name,
            //    Timestamp = DateTime.Now,
            //    IsSentByCurrentUser = true,

            //});
            SaveMessagesToDatabase() ;
            CurrentMessage = string.Empty;
        }

        private bool CanSendMessage() => !string.IsNullOrWhiteSpace(CurrentMessage);

        private void UploadFile()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Music Files (*.mp3)|*.mp3", Title = "Select a File" };
            if (openFileDialog.ShowDialog() == true)
            {
                SendFile(openFileDialog.FileName);
            }
        }

        private void SendFile(string filePath)
        {
            Messages.Add(new ChatMessage
            {
                Content = $"File: {Path.GetFileName(filePath)}",
                FilePath = filePath,
                Timestamp = DateTime.Now,
                IsSentByCurrentUser = true,
                ReceiverName= SelectedPeer.Name
            });

            SaveMessagesToDatabase();
        }

        private void CreateGroup()
        {
            string groupName = Prompt.ShowDialog("Enter group name:", "Create Group");
            if (!string.IsNullOrEmpty(groupName))
            {
                var newGroup = new Group { GroupName = groupName };
                Groups.Add(newGroup);
                _groupManager.CreateGroup(groupName);
            }
        }

        private void RenameGroup()
        {
            if (SelectedGroup != null)
            {
                string newName = Prompt.ShowDialog("Enter new group name:", "Rename Group");
                if (!string.IsNullOrEmpty(newName))
                {
                    _groupManager.RenameGroup(SelectedGroup.GroupName, newName);
                    SelectedGroup.GroupName = newName;
                }
            }
        }

        private void AddPeerToGroup(Peer peer)
        {
            if (SelectedGroup != null && !SelectedGroup.Members.Contains(peer))
            {
                SelectedGroup.Members.Add(peer);
                _groupManager.AddPeerToGroup(SelectedGroup.GroupName, peer);
            }
        }

        private void SendHeartbeat(object sender, ElapsedEventArgs e)
        {
         

            foreach (var peer in Peers)
            {
                peerDiscovery.StartBroadcasting();
                peer.Status = PeerStatus.Online; // Simulated check

            }
        }

        private void SaveMessagesToDatabase()
        {
            string connectionString = "Data Source=messages.db;Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Messages (SenderName TEXT, Content TEXT, Timestamp TEXT)", connection))
                {
                    command.ExecuteNonQuery();
                }

                foreach (var message in Messages)
                {
                    using (var command = new SQLiteCommand("INSERT INTO Messages (SenderName, Content, Timestamp) VALUES (@SenderName, @Content, @Timestamp)", connection))
                    {
                        if(message.IsSentByCurrentUser == true)
                        {
                            message.SenderName = Environment.MachineName;
                        }
                        command.Parameters.AddWithValue("@SenderName", message.SenderName);
                        command.Parameters.AddWithValue("@Content", _aesEncryption.Encrypt( message.Content)); // Encrypt message
                        command.Parameters.AddWithValue("@Timestamp", message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        private void LoadMessagesFromDatabase()
        {
            string connectionString = "Data Source=messages.db;Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT SenderName, Content, Timestamp FROM Messages", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string encryptedContent = reader["Content"]?.ToString();

                        // 🔍 Check if Content is NULL or empty before decrypting
                        if (string.IsNullOrEmpty(encryptedContent))
                        {
                            Console.WriteLine("Skipping empty message...");
                            continue; // Skip empty messages
                        }

                        try
                        {
                            string decryptedContent = _aesEncryption.Decrypt(encryptedContent);
                            Messages.Add(new ChatMessage
                            {
                                SenderName = reader["SenderName"].ToString(),
                                Content = decryptedContent,
                                Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Decryption failed: {ex.Message}");
                            Messages.Add(new ChatMessage
                            {
                                SenderName = reader["SenderName"].ToString(),
                                Content = "[Decryption Error]",
                                Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                            });
                        }
                    }
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
