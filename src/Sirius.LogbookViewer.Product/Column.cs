using System;
using System.Collections.Generic;

namespace Sirius.LogbookViewer.Product
{
    public class Column
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public bool Filter { get; set; }
        public bool Sortable { get; set; }
        public bool IsIndex { get; set; }
        public bool UseIcon { get; set; }
        public Dictionary<string, string> IconData { get; set; }
    }
}
