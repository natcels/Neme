using Microsoft.Xaml.Behaviors.Media;
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
    /// Interaction logic for Sidebar.xaml
    /// </summary>
    public partial class Sidebar : UserControl
    {
        private bool isCollapsed = true;
        public Sidebar()
        {
            InitializeComponent();
        }

        private void ToggleSidebar(object sender, RoutedEventArgs e)
        {
            if (isCollapsed) {
                
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

        private void Navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (NavigationMenu.SelectedItem is ListBoxItem)
            //{
            //    if (this.Parent is MainView parentPanel)
            //    {
            //        parentPanel.ShowChild(NavigationMenu.SelectedIndex);
            //    }
            //}
            if(this.Parent is MainView ParentPanel)
            {
                ParentPanel.ShowChild(NavigationMenu.SelectedIndex);
            }
        }
    }
}
