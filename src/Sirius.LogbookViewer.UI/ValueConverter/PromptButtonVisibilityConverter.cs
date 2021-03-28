using System;
using System.Globalization;
using System.Windows;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Converts the prompt status (<see cref="bool"/>) of a window into <see cref="Visibility"/> to display a prompt button on the window or not.
    /// </summary>
    class PromptButtonVisibilityConverter : BaseValueConverter<PromptButtonVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return Visibility.Hidden;
            }

            return ((bool)value) == true ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
