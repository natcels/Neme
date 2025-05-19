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

        public User User { get; set; }
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
            if (string.IsNullOrWhiteSpace(User.Username) || string.IsNullOrWhiteSpace(User.Password) || string.IsNullOrWhiteSpace(User.Department))
            {
                MessageBox.Show("All fields (except Email) are required!", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserService.UserExists(User.Username))
            {
                MessageBox.Show("Username already taken!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                Username = User.Username,
                Password = User.Password,
                Email = User.Email,
                Department = User.Department,
                AvatarPath = User.AvatarPath
            };

            UserService.AddUser(newUser);
            MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void LoginUser()
        {
            if (string.IsNullOrWhiteSpace(User.Username) || string.IsNullOrWhiteSpace(User.Password))
            {
                MessageBox.Show("Enter your Username and password!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserService.ValidateUser(User.Username, User.Password))
            {
                //var user = UserService.GetUser(Username);
                //AvatarPath = user.AvatarPath;
                
                MessageBox.Show($"Welcome, {User.Username}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Invalid Username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                User.AvatarPath = openFileDialog.FileName;
            }
        }



    }

}

