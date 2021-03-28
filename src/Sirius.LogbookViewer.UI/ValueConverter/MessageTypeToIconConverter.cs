using System;
using System.Diagnostics;
using System.Globalization;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Converts a message type ID to a relevant icon.
    /// </summary>
    class MessageTypeToIconConverter : BaseValueConverter<MessageTypeToIconConverter>
    {
        private const string ICONS_URI = "pack://application:,,,/Sirius.LogbookViewer.UI;component/Resource/Icon/";
        private const string ICON_ERROR = "error.png";
        private const string ICON_OPERATING_ERROR = "fault.png";
        private const string ICON_WARNING = "trip.png";
        private const string ICON_PREWARNING = "prewarn.png";
        private const string ICON_EVENT = "error.png";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int messageType = (int) value;

            switch (messageType)
            {
                case 1: return ICONS_URI + ICON_ERROR;
                case 2: return ICONS_URI + ICON_OPERATING_ERROR;
                case 3: return ICONS_URI + ICON_WARNING;
                case 4: return ICONS_URI + ICON_PREWARNING;
                case 5: return ICONS_URI + ICON_EVENT;
                default:
                    Debug.Fail($"Unknown message type: {messageType}");
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
