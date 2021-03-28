using System.Collections.Generic;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Column & row data for the exported file.
    /// </summary>
    public class FileMeta
    {
        public List<string> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
    }
}
