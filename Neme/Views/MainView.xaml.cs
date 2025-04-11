using Neme.helpers;
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

namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private ChatView ChatView;
        private Addressbook Addressbook;
        private CalendarView CalendarView;
        private KanbanView KanbanView;
        private Calls CallsView;
        private ScreenShareWindow Collaboration;
        private SettingsWindow SettingsWindow;

        public MainView()
        {
            Addressbook = new Addressbook();
            ChatView = new ChatView();
            CalendarView = new CalendarView();
            KanbanView = new KanbanView();
            CallsView = new Calls();
            Collaboration = new ScreenShareWindow();
            SettingsWindow = new SettingsWindow();

            InitializeComponent();
        }

        public void ShowChild(int childId)
        {
            UIElement newChild = childId switch
            {
                0 => Addressbook,
                1 => ChatView,
                2 => CalendarView,
                3 => KanbanView,
                4 => Collaboration,
                5 => CallsView,
                6 => SettingsWindow,
                _ => ChatView,
            };

            if (ChildArea.Children.Count > 0 && ChildArea.Children[0] == newChild)
                return;

            //ChildArea.Children.Clear();
            //ChildArea.Children.Add(newChild);

            ChildArea.SwapWithFade(newChild);
        }


        private void ToggleNotificationsMenu(object sender, RoutedEventArgs e)
        {
            NotificationsPopup.IsOpen = !NotificationsPopup.IsOpen;
        }

        private void ClearNotifications(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("All notifications cleared!");
            NotificationsPopup.IsOpen = false;
        }

        private void ToggleSettingsMenu(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }

        private void OpenThemeSettings(object sender, RoutedEventArgs e)
        {
            ShowChild(6);
        }

        private void OpenAccountSettings(object sender, RoutedEventArgs e)
        {
            ShowChild(6);
        }

        private void OpenPreferences(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open Preferences...");
        }

        private void LogoutUser(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logging out...");
        }

    }
}
