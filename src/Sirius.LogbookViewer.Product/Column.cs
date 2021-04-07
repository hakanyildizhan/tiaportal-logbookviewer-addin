using System;
using System.Collections.Generic;

namespace Sirius.LogbookViewer.Product
{
    public class Column
    {
        /// <summary>
        /// Data type of this column.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Column name. This will show up in the grid column header.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if the grid is filterable based on this column. Only one column can be set as filterable.
        /// </summary>
        public bool Filter { get; set; }

        /// <summary>
        /// Options for displaying the filters.
        /// </summary>
        public Dictionary<string, FilterOption> FilterData { get; set; }

        /// <summary>
        /// Whether this column is sortable by clicking the grid column header.
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// Whether this column is the Index column. Index columns are always re-enumerated after filtering and sorting.
        /// </summary>
        public bool IsIndex { get; set; }

        /// <summary>
        /// In case the column data is empty and cannot be cast to the column type, this string will be shown on the grid.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Whether data for this column will be icons.
        /// </summary>
        public bool UseIcon { get; set; }

        /// <summary>
        /// If <see cref="UseIcon"/> is true, the key-value pairs to look up the column data and relative path for the icon.
        /// </summary>
        public Dictionary<string, string> IconData { get; set; }
    }
}
