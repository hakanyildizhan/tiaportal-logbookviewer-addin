using System;
using System.Globalization;

namespace Sirius.LogbookViewer.UI
{
    class FilterCommandParameterConverter : BaseMultiValueConverter<FilterCommandParameterConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is bool) || !(values[1] is string))
            {
                return false;
            }

            return new FilterCommandParameter() { ColumnValue = values[1] as string, Selected = (bool)values[0] };
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
