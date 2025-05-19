using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Neme.Services;

namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for InviteUser.xaml   
    /// </summary>
    public partial class InviteUser : Window
    {
        private Guid _fileId;
        public InviteUser(Guid FileId)
        {
            InitializeComponent();
            _fileId = FileId;
        }

        private void InviteButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a valid Username.");
                return;
            }

            if (!UserService.UserExists(username))
            {
                MessageBox.Show("User does not exist.");
                return;
            }

            FileService.AddCollaborator(_fileId, username);
            MessageBox.Show($"{username} has been invited.");
            this.Visibility = Visibility.Collapsed;
            
        }
    }
}
