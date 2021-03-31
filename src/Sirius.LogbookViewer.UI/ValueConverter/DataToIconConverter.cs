using System;
using System.Globalization;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Converts data to a relevant icon.
    /// </summary>
    class DataToIconConverter : BaseValueConverter<DataToIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueToConvert = value.ToString();
            var param = ((DataToIconConverterParameter)parameter);
            return param.IconData.ContainsKey(valueToConvert) ?
                $"pack://application:,,,/{param.AssemblyName};component/{param.IconData[valueToConvert]}" :
                null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
