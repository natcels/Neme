using Neme.Utils;
using System;
using System.Windows;
using Neme.ViewModels;
using Neme.Helpers;
using Neme.Models;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using NAudio.CoreAudioApi.Interfaces;
using Neme.helpers;
using MaterialDesignThemes.Wpf;

namespace Neme.Views
{
    public partial class MainWindow : Window
    {
        public User user;        
        private PeerManager peerManager;
        private TcpListenerHandler tcpListenerHandler;
        private TcpClientHandler tcpClientHandler;
        private WebSocketClient _client;
        private int port;
        private string broadcastAddress;
        private string encryptionKey;
        public string Username;
        public string machineName;

        public MainWindow()
        {
            InitializeComponent();
            machineName = Environment.MachineName;
           
            DataContext = new MainViewModel();

           
            
            // Initialize settings
            InitializeSettings();
            
            // Initialize components
            ShowLogin();
            InitializeDatabase();

        }

        private void ShowLogin()
        {
            var loginView = new AuthView();
            loginView.LoginSucceeded += OnLoginSuccess;
            MainContent.Content = loginView;

        }

        private void OnLoginSuccess(object sender, EventArgs e)
        {
            ShowMainApp();
        }

        public void ShowMainApp()
        {
            MainContent.Content = new MainView(); // Your main app
        }

        private void InitializeSettings()
        {
            encryptionKey = Environment.MachineName;
            port = 9000;
            broadcastAddress = "192.168.1.107";
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
