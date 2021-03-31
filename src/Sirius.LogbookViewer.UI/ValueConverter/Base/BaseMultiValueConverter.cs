using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sirius.LogbookViewer.UI
{
    public abstract class BaseMultiValueConverter<T> : MarkupExtension, IMultiValueConverter
        where T : class, new()
    {
        private static T Converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Converter ?? (Converter = new T());
        }

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}
