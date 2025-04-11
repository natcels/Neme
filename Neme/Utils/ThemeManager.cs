using System.Windows;
using System.Windows.Media;

namespace Neme.Utils
{
    public static class ThemeManager
    {
        public static void SetTheme(bool isDarkMode)
        {
            Application.Current.Resources["SidebarBackground"] = isDarkMode ? Brushes.Black : Brushes.White;
            Application.Current.Resources["SidebarText"] = isDarkMode ? Brushes.White : Brushes.Black;
        }

        public static void SetAccentColor(bool useTeal)
        {
            Application.Current.Resources["AccentColor"] = useTeal ? Brushes.Teal : Brushes.Navy;
        }
    }
}
