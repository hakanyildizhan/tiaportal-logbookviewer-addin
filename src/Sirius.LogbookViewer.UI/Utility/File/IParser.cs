using Sirius.LogbookViewer.UI.Model;
using System.Collections.Generic;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Interface for product-specific parser implementations.
    /// </summary>
    public interface IParser
    {
        IList<LogbookMessage> Parse();
    }
}
