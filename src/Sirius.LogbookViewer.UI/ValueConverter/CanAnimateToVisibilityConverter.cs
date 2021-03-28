using System;
using System.Globalization;
using System.Windows;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Converts the animation status (<see cref="bool"/>) into <see cref="Visibility"/> to display an animated icon or a still image.
    /// </summary>
    class CanAnimateToVisibilityConverter : BaseValueConverter<CanAnimateToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return Visibility.Hidden;
            }

            if (parameter == null)
                return ((bool)value) == true ? Visibility.Visible : Visibility.Hidden;
            else
                return ((bool)value) == false ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
