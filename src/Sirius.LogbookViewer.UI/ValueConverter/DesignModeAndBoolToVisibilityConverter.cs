using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Ands whether the context is design mode and whether the element should be shown to finally show or hide the element.
    /// </summary>
    class DesignModeAndBoolToVisibilityConverter : BaseMultiValueConverter<DesignModeAndBoolToVisibilityConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is DependencyObject) || !(values[1] is bool))
            {
                return Visibility.Hidden;
            }

            return !DesignerProperties.GetIsInDesignMode((DependencyObject)values[0]) && (bool)values[1] ? Visibility.Visible : Visibility.Hidden;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
