using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SystemTroubleShooter.Helpers.Converters
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
                    return Brushes.LimeGreen; 
                case "offline":
                case "failed":
                    return Brushes.OrangeRed; 
                case "pending":
                case "running":
                    return Brushes.Gold; 
                case "info":
                    return Brushes.LightSkyBlue; 
                case "unknown":
                default:
                    return Brushes.Gray; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
