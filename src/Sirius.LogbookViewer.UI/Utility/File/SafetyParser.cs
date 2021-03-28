using System.Collections.Generic;
using Sirius.LogbookViewer.UI.Model;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Safety specific parser for logbook exports.
    /// </summary>
    public class SafetyParser : IParser
    {
        private readonly FileMeta _fileMeta;

        public SafetyParser(FileMeta fileMeta)
        {
            _fileMeta = fileMeta;
        }

        public IList<LogbookMessage> Parse()
        {
            var messageList = new List<LogbookMessage>();

            foreach (var parts in _fileMeta.Rows)
            {
                var message = new SafetyLogbookMessage();

                // column 0 - index
                bool valid = int.TryParse(parts[0], out int messageIndex);

                if (!valid)
                {
                    continue;
                }

                message.Index = messageIndex;

                // column 1 - message type (ES) OR type ID (Classic)
                bool isNumber = int.TryParse(parts[1], out int messageType);

                if (!isNumber)
                {
                    message.Type = GetMessageTypeCode(parts[1].Trim());
                }
                else
                {
                    message.Type = messageType;
                }
                
                // column 2 - source number (Classic) OR source (ES)
                isNumber = int.TryParse(parts[2], out int sourceNumber);

                if (!isNumber)
                {
                    message.Source = parts[2].Trim();
                }
                else
                {
                    message.Source = GetSource(sourceNumber);
                }
                
                // column 3 - operating hours
                message.OperatingHours = parts[3].Trim();

                // column 4 - element number
                message.ElementNumber = string.IsNullOrEmpty(parts[4]) ? "-" : parts[4].Trim();

                // column 5 - element function
                message.ElementFunction = string.IsNullOrEmpty(parts[5]) ? "-" : parts[5].Trim();

                // column 6 - object number
                valid = int.TryParse(parts[6], out int objectNumber);

                if (!valid)
                {
                    continue;
                }

                message.ObjectNumber = objectNumber;

                // column 7 - message
                message.Message = string.IsNullOrEmpty(parts[6]) ? "-" : parts[7].Trim();

                messageList.Add(message);
            }

            return messageList;
        }

        private string GetSource(int sourceNumber)
        {
            switch (sourceNumber)
            {
                case 1: return "Device";
                case 2: return "Elements";
                case 3: return "Communication";
                case 4: return "Product-specific";
                default: return "Unknown";
            }
        }

        private int GetMessageTypeCode(string messageType)
        {
            switch (messageType)
            {
                case "Error": return 1;
                case "Operating Error": return 2;
                case "Warning": return 3;
                case "Prewarning": return 4;
                case "Event": return 5;
                default: return -1;
            }
        }
    }
}
