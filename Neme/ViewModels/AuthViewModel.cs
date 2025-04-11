using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BCrypt.Net;
using Microsoft.Win32;
using Neme.Services;
using Neme.Utils;
using Neme.Models;

namespace Neme.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _department;
        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged(nameof(Department));
            }
        }

        private string? _avatarPath;
        public string? AvatarPath
        {
            get => _avatarPath;
            set
            {
                _avatarPath = value;
                OnPropertyChanged(nameof(AvatarPath));
            }
        }

        public RelayCommand RegisterCommand { get; }
        public RelayCommand LoginCommand { get; }
        public RelayCommand SelectAvatarCommand { get; }

        public AuthViewModel()
        {
            RegisterCommand = new RelayCommand(RegisterUser);
            LoginCommand = new RelayCommand(LoginUser);
            SelectAvatarCommand = new RelayCommand(SelectAvatar);
        }

        private void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Department))
            {
                MessageBox.Show("All fields (except Email) are required!", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserService.UserExists(Username))
            {
                MessageBox.Show("Username already taken!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                Username = Username,
                PasswordHash = Password,
                Email = Email,
                Department = Department,
                AvatarPath = AvatarPath
            };

            UserService.AddUser(newUser);
            MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoginUser()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Enter your username and password!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserService.ValidateUser(Username, Password))
            {
                var user = UserService.GetUser(Username);
                AvatarPath = user.AvatarPath;

                MessageBox.Show($"Welcome, {user.Username}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectAvatar()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Avatar",
                Filter = "Image Files|*.jpg;*.png;*.jpeg",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AvatarPath = openFileDialog.FileName;
            }
        }

    }

}

