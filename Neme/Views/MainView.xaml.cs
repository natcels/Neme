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
using System.Windows.Media.Animation;
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
        private Addressbook AddressBook;
        private CalendarView CalendarView;
        private KanbanView KanbanView;
        private Calls CallsView;
        private Collaboration Collaboration;
        private SettingsWindow SettingsWindow;
        public string Username = "";
        private bool isCollapsed = false;


        public MainView()
        {
            AddressBook = new Addressbook();
            ChatView = new ChatView();
            CalendarView = new CalendarView();
            KanbanView = new KanbanView();
            CallsView = new Calls();
            Collaboration = new Collaboration();
            SettingsWindow = new SettingsWindow();
            
            InitializeComponent();
            getUsername();
        }

        private void getUsername()
        {
            if(this.Parent is MainWindow mainWindow)
            {
                this.Username = mainWindow.Username;
            }
        }

        public void ShowChild(int childId)
        {
            UIElement newChild = childId switch
            {
                0 => AddressBook,
                1 => ChatView,
                2 => CalendarView,
                3 => KanbanView,
                4 => Collaboration,
                5 => CallsView,
                6 => SettingsWindow,
                _ => AddressBook,
            };

            if (ChildArea.Children.Count > 0 && ChildArea.Children[0] == newChild)
                return;
            //  ChildArea.SwapWithFade(newChild);
            ChildArea.Children.Clear();
            ChildArea.Children.Add(newChild);
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

        private void ToggleSidebar(object sender, RoutedEventArgs e)
        {
            if (isCollapsed)
            {

                BeginStoryboard((Storyboard)FindResource("ExpandSidebar"));
                ChatsLabel.Visibility = Visibility.Visible;
                CalendarLabel.Visibility = Visibility.Visible;
                SettingsLabel.Visibility = Visibility.Visible;
                MeetingsLabel.Visibility = Visibility.Visible;
                TaskLabel.Visibility = Visibility.Visible;
                KanbanLabel.Visibility = Visibility.Visible;
                ContactsLabel.Visibility = Visibility.Visible;
            }
            else
            {
                BeginStoryboard((Storyboard)FindResource("CollapseSidebar"));
                ChatsLabel.Visibility = Visibility.Collapsed;
                CalendarLabel.Visibility = Visibility.Collapsed;
                SettingsLabel.Visibility = Visibility.Collapsed;
                MeetingsLabel.Visibility = Visibility.Collapsed;
                TaskLabel.Visibility = Visibility.Collapsed;
                KanbanLabel.Visibility = Visibility.Collapsed;
                ContactsLabel.Visibility = Visibility.Collapsed;


            }
            isCollapsed = !isCollapsed;
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
            ShowChild(6);
        }

        private void LogoutUser(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logging out...");
        }
        private void Navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowChild(NavigationMenu.SelectedIndex);
        }
    }
}
