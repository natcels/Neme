using System.ComponentModel;
using System.Runtime.CompilerServices;
using BCrypt.Net;

namespace Neme.Models
{
    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _username;
        private string _password;
        private string? _email;
        private string _department;
        private string _avatarPath; // Stores avatar image path

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string? Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(); }
        }


        public string AvatarPath
        {
            get => _avatarPath;
            set { _avatarPath = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

      

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }
    }
}
