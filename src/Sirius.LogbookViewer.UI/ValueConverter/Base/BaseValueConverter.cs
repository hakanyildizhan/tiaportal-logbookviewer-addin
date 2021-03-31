using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// A wrapper for value converter implementations that makes them usable in XAML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        private static T Converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Converter ?? (Converter = new T());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
