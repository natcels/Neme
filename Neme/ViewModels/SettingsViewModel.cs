using Neme.Models;
using Neme.Services;
using Neme.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Neme.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private UserSettings _settings;
        public string DisplayName
        {
            get => _settings.DisplayName;
            set { _settings.DisplayName = value; OnPropertyChanged(nameof(DisplayName)); }
        }
        public string Status
        {
            get => _settings.Status;
            set { _settings.Status = value; OnPropertyChanged(nameof(Status)); }
        }
        public string AvatarImage
        {
            get => _settings.AvatarImage;
            set { _settings.AvatarImage = value; OnPropertyChanged(nameof(AvatarImage)); }
        }
        public bool IsLightTheme
        {
            get => _settings.IsLightTheme;
            set { _settings.IsLightTheme = value; OnPropertyChanged(nameof(IsLightTheme)); OnPropertyChanged(nameof(IsDarkTheme)); }
        }

        public bool IsDarkTheme => !IsLightTheme;

        public bool EnableNotifications
        {
            get => _settings.EnableNotifications;
            set { _settings.EnableNotifications = value; OnPropertyChanged(nameof(EnableNotifications)); }
        }
        public bool PlaySound
        {
            get => _settings.PlaySound;
            set { _settings.PlaySound = value; OnPropertyChanged(nameof(PlaySound)); }
        }

        public string Port
        {
            get => _settings.Port;
            set { _settings.Port = value; OnPropertyChanged(nameof(Port)); }
        }

        public string EncryptionKey
        {
            get => _settings.EncryptionKey;
            set { _settings.EncryptionKey = value; OnPropertyChanged(nameof(EncryptionKey)); }
        }

        public ICommand ChangeAvatarCommand { get; }
        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            _settings = SettingsService.LoadSettings() ?? new UserSettings
            {
                DisplayName = "User",
                Status = "Available",
                AvatarImage = "Images/default.png",
                IsLightTheme = true,
                EnableNotifications = true,
                PlaySound = true,
                Port = "9876",
                EncryptionKey = Environment.MachineName
            };

            ChangeAvatarCommand = new RelayCommand(ChangeAvatar);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void ChangeAvatar()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                AvatarImage = dialog.FileName;
            }
        }

        private void SaveSettings()
        {
            SettingsService.SaveSettings(_settings);
            MessageBox.Show("Settings saved!");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
