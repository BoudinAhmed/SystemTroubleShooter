using System;
using System.Globalization;
using System.Windows; // Required for Visibility
using System.Windows.Data; // *** REQUIRED for IValueConverter ***

namespace SystemTroubleShooter.Helpers.Converters // Make sure this namespace matches your project structure
{
    /// <summary>
    /// Converter to convert a string's emptiness/nullity to Visibility.
    /// Returns Visible if the string is null or whitespace, otherwise returns Collapsed.
    /// Used to show a placeholder text when a TextBox is empty.
    /// </summary>
    // *** Ensure you implement IValueConverter here ***
    public class StringToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string value to a Visibility value.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The type of the target property (should be Visibility).</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture to use (not used).</param>
        /// <returns>Visible if the string is null or whitespace, otherwise Collapsed.</returns>
        // *** Ensure the method signature is exactly like this ***
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

        /// <summary>
        /// ConvertBack is not implemented as this is a one-way converter.
        /// </summary>
        // *** Ensure the method signature is exactly like this ***
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This is a one-way converter and ConvertBack is not needed.");
        }
    }
}