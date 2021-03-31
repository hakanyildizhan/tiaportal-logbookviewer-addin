using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Converter to make a UI element visible in Designer mode.
    /// </summary>
    public class DesignModeToVisibilityConverter : BaseValueConverter<DesignModeToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return DesignerProperties.GetIsInDesignMode((DependencyObject)value) ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                return DesignerProperties.GetIsInDesignMode((DependencyObject)value) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
