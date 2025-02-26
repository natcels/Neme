using Neme.Utils;
using System;
using System.Windows;
using Neme.ViewModels;
using Neme.Models;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Data.SQLite;

namespace Neme.Views
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
