using cleddmessenger.Utils;
using System;
using System.Windows;
using cleddmessenger.ViewModels;
using cleddmessenger.Models;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Data.SQLite;

namespace cleddmessenger.Views
{
    public partial class MainWindow : Window
    {
        private PeerManager peerManager;
        private TcpListenerHandler tcpListenerHandler;
        private TcpClientHandler tcpClientHandler;
        private WebSocketClient _client;
        private int port;
        private string broadcastAddress;
        private string encryptionKey;

        public MainWindow()
        {
            DataContext = new MainViewModel();

            // Initialize settings
            InitializeSettings();
           
            
            // Initialize components
            InitializeComponents();

            InitializeDatabase();

        }

        private void InitializeSettings()
        {
            encryptionKey = Environment.MachineName;
            port = 9000;
            broadcastAddress = "192.168.1.107";
        }

        private void InitializeComponents()
        {
            try
            {
                // Initialize PeerManager for discovery and heartbeat
                peerManager = new PeerManager(broadcastAddress, port, encryptionKey);

                // Initialize TCP listener for incoming messages
                tcpListenerHandler = new TcpListenerHandler(port + 1);
                tcpListenerHandler.StartListening();

                // Initialize TCP client for outgoing messages
                tcpClientHandler = new TcpClientHandler("127.0.0.1", port + 1, encryptionKey);

                // Initialize WebSocket client for signaling
                _client = new WebSocketClient();
                _client.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing components: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartBroadcasting()
        {
            try
            {
                peerManager?.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start broadcasting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendMessage(string message)
        {
            try
            {
                tcpClientHandler?.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DisplayMessages(List<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                // Create a new ListBoxItem
                var listItem = new ListBoxItem();
                listItem.DataContext = message;

                // Check if the message contains a file
                if (!string.IsNullOrEmpty(message.FilePath))
                {
                    // Set file display properties
                    var filePanel = new StackPanel { Orientation = Orientation.Horizontal };

                    // If the file is an image, show an image preview
                    if (message.Type == MessageType.Image)
                    {
                        var imagePreview = new Image
                        {
                            Source = new BitmapImage(new Uri(message.FilePath)),
                            Width = 200,
                            Height = 200
                        };
                        filePanel.Children.Add(imagePreview);
                    }
                    // If the file is a regular document, show the file name
                    else if (message.Type == MessageType.File)
                    {
                        var fileText = new TextBlock { Text = $"File: {message.FileName}" };
                        filePanel.Children.Add(fileText);
                        var openButton = new Button { Content = "Open" };
                        openButton.Click += (sender, e) => OpenFile(message.FilePath);
                        filePanel.Children.Add(openButton);
                    }

                    listItem.Content = filePanel;
                }
                // Add normal message content if it's text-based
                else
                {
                    var textBlock = new TextBlock { Text = message.Content, TextWrapping = TextWrapping.Wrap };
                    listItem.Content = textBlock;
                }

                // Add the ListItem to the ListBox
                ChatMessagesList.Items.Add(listItem);
                

            }
        }

      
        // Handle file opening (for file and image messages)
        private void OpenFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=messages.db;Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Messages (Id INTEGER PRIMARY KEY AUTOINCREMENT, SenderName TEXT NOT NULL, Content TEXT NOT NULL, Timestamp TEXT NOT NULL)",
                    connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        

 

       

        private void Window_Closed(object sender, EventArgs e)
        {
            // Graceful shutdown
            _client?.CloseConnection();
            peerManager?.Stop();
            tcpListenerHandler?.StopListening();
        }

    
    }
}
