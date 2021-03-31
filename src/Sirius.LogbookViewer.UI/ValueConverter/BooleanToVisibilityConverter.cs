using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Converter to make a UI element visible if given parameter value is true.
    /// </summary>
    public class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
