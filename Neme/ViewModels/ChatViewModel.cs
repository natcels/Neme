using Neme.Models;
using Neme.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace Neme.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public ObservableCollection<ChatMessage> Messages { get; private set; }

        public ChatViewModel()
        {
            Messages = new ObservableCollection<ChatMessage>();
            LoadMessagesFromDatabase();
        }

        public void UploadFile(Peer peer)
        {
            using (var openFileDialog = new OpenFileDialog { Filter = "All Files|*.*", Title = "Select a File" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SendFile(openFileDialog.FileName, peer);
                }
            }
        }

        private void SendFile(string filePath, Peer selectedPeer)
        {
            if (selectedPeer == null) return;

            var message = new ChatMessage
            {
                FilePath = filePath,
                Timestamp = DateTime.Now,
                IsSentByCurrentUser = true,
                ReceiverName = selectedPeer.Name
            };

            Messages.Add(message);
            SaveMessagesToDatabase();
        }

        public void SendMessage(string messageContent, Peer selectedPeer = null, Group selectedGroup = null)
        {
            if (string.IsNullOrWhiteSpace(messageContent)) return;

            if (selectedGroup != null)
            {
                foreach (var peer in selectedGroup.Members)
                {
                    SendMessageToPeer(messageContent, peer);
                }
            }
            else if (selectedPeer != null)
            {
                SendMessageToPeer(messageContent, selectedPeer);
            }
        }

        private void SendMessageToPeer(string message, Peer selectedPeer)
        {
            var encryptedMessage = AesEncryption.Encrypt(message);

            var newMessage = new ChatMessage
            {
                EncryptedContent = encryptedMessage,
                ReceiverName = selectedPeer?.Name,
                Timestamp = DateTime.Now,
                IsSentByCurrentUser = true
            };

            Messages.Add(newMessage);
            SaveMessagesToDatabase();
        }

        public void SaveMessagesToDatabase()
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
                        command.Parameters.AddWithValue("@SenderName", message.IsSentByCurrentUser ? Environment.MachineName : message.ReceiverName);
                        command.Parameters.AddWithValue("@Content", message.EncryptedContent);
                        command.Parameters.AddWithValue("@Timestamp", message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void LoadMessagesFromDatabase()
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
                        try
                        {
                            string decryptedContent = AesEncryption.Decrypt(reader["Content"].ToString());
                            Messages.Add(new ChatMessage
                            {
                                SenderName = reader["SenderName"].ToString(),
                                EncryptedContent = decryptedContent,
                                Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Decryption failed: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
