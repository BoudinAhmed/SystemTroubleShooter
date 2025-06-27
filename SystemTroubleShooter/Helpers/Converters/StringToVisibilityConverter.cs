using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data; 

namespace SystemTroubleShooter.Helpers.Converters 
{
    
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is a string and if it's null or whitespace
            if (value is string text)
            {
                // Show the element (e.g., placeholder TextBlock) if the string is null or whitespace
                return string.IsNullOrWhiteSpace(text) ? Visibility.Visible : Visibility.Collapsed;
            }

            // If the value is not a string (e.g., null binding), default to visible
            return Visibility.Visible;
        }

        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This is a one-way converter and ConvertBack is not needed.");
        }
    }
}