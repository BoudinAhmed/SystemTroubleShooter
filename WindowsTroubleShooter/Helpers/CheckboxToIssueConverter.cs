using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindowsTroubleShooter.Helpers
{
    public class CheckboxToIssueConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var issues = value as ObservableCollection<string>;
            var issue = parameter as string;

            return issues?.Contains(issue) ?? false; // Return true if the issue is in the list, otherwise false
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isChecked = (bool)value;
            var issue = parameter as string;

            return isChecked ? new ObservableCollection<string> { issue } : new ObservableCollection<string>();
        }
    }
}
