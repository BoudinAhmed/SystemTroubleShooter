using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WindowsTroubleShooter.Helpers.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value?.ToString()?.ToLowerInvariant())
            {
                case "online":
                case "fixed":
                case "success":
                    return Brushes.LimeGreen; // Or a lighter green like #FF90EE90
                case "offline":
                case "failed":
                    return Brushes.OrangeRed; // Or a color like #FFFF6347
                case "pending":
                case "running":
                    return Brushes.Gold; // Or #FFFFD700
                case "info":
                    return Brushes.LightSkyBlue; // Or #FF87CEFA
                case "unknown":
                default:
                    return Brushes.Gray; // Or #FFAAAAAA
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
