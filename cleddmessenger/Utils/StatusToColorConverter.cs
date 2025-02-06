using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Neme.Utils
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PeerStatus status)
            {
                return status switch
                {
                    PeerStatus.Active => Brushes.Green,
                    PeerStatus.Offline => Brushes.Gray,
                    PeerStatus.Unresponsive => Brushes.Red,
                    _ => Brushes.Transparent
                };
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
