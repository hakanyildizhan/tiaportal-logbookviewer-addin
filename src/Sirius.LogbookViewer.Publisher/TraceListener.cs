using System;
using System.Diagnostics;

namespace Sirius.LogbookViewer.Publisher
{
    /// <summary>
    /// Used for outputting application messages to the console. See App.config file for details.
    /// </summary>
    public class ConsoleListener : ConsoleTraceListener
    {
        public ConsoleListener() : base() { }

        public override void Write(string message)
        {
            base.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffffff ") + message);
        }
    }
}
