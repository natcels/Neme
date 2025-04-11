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
using Neme.ViewModels;


namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for AuthView.xaml
    /// </summary>
    public partial class AuthView : UserControl
    {
        public event EventHandler LoginSucceeded;
        public AuthView()
        {
            DataContext =new AuthViewModel();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Fake validation (you’ll replace this with real logic)
            if (UsernameText.Text == "admin" && PasswordText.Password == "1234")
            {
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Invalid credentials");
            }
        }
    }
}
