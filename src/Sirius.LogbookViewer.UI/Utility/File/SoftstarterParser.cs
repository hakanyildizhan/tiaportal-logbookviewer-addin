using Sirius.LogbookViewer.UI.Model;
using System;
using System.Collections.Generic;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Softstarter specific parser for logbook exports.
    /// </summary>
    public class SoftstarterParser : IParser
    {
        private readonly FileMeta _fileMeta;

        public SoftstarterParser(FileMeta fileMeta)
        {
            _fileMeta = fileMeta;
        }

        public IList<LogbookMessage> Parse()
        {
            var messageList = new List<LogbookMessage>();

            foreach (var parts in _fileMeta.Rows)
            {
                var message = new SoftstarterLogbookMessage();

                // column 0 - source
                message.Source = parts[0].Trim();

                // column 1 - message
                message.Message = parts[1].Trim();

                // column 2 - operating hours
                message.OperatingHours = parts[2].Trim();

                // column 3 - date
                bool isValid = DateTime.TryParse(parts[3].Trim(), out DateTime date);

                if (!isValid)
                {
                    continue;
                }

                message.Date = date;

                // column 4 - time
                message.Time = parts[4].Trim();

                // column 5 - identification / object number
                isValid = int.TryParse(parts[5], out int objectNumber);

                if (!isValid)
                {
                    continue;
                }

                message.ObjectNumber = objectNumber;

                messageList.Add(message);
            }

            return messageList;
        }
    }
}
