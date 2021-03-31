using System.Collections.Generic;

namespace Sirius.LogbookViewer.Product
{
    public class LogbookData
    {
        public List<Column> ColumnData { get; set; }
        public List<List<object>> RowData { get; set; }
    }
}
