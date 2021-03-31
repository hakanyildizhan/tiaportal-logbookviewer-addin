using Sirius.LogbookViewer.Product;
using System.Collections.Generic;

namespace Sirius.LogbookViewer.UI.Model
{
    /// <summary>
    /// A sample list of Safety specific logbook messages for Safety Logbook grid.
    /// </summary>
    public class MessageListDesignModelSafety : GridViewModel
    {
        public static MessageListDesignModelSafety Instance => new MessageListDesignModelSafety();

        public MessageListDesignModelSafety() : base()
        {
            var columnData = new List<Column>();
            columnData.Add(new Column() { Type = typeof(int), Name = "Index", IsIndex = true });
            columnData.Add(new Column()
            {
                Type = typeof(int),
                Name = "Type",
                Filter = true,
                FilterData = new Dictionary<string, FilterOption>()
                {
                    { "1", new FilterOption() { DisplayValue = "Error", IconPath = "Resources/Icon/error.png" } },
                    { "2", new FilterOption() { DisplayValue = "Operating Error", IconPath = "Resources/Icon/fault.png" } },
                    { "3", new FilterOption() { DisplayValue = "Warning", IconPath = "Resources/Icon/trip.png" } },
                    { "4", new FilterOption() { DisplayValue = "Prewarning", IconPath = "Resources/Icon/prewarn.png" } },
                    { "5", new FilterOption() { DisplayValue = "Event", IconPath = "Resources/Icon/event.png" } }
                },
                Sortable = true,
                UseIcon = true,
                IconData = new Dictionary<string, string>()
                {
                    { "1", "Resources/Icon/error.png" },
                    { "2", "Resources/Icon/fault.png" },
                    { "3", "Resources/Icon/trip.png" },
                    { "4", "Resources/Icon/prewarn.png" },
                    { "5", "Resources/Icon/event.png" }
                }
            });
            columnData.Add(new Column() { Type = typeof(string), Name = "Source", Sortable = true });
            columnData.Add(new Column() { Type = typeof(string), Name = "Operating Hours", Sortable = true });
            columnData.Add(new Column() { Type = typeof(int), Name = "Element Number" });
            columnData.Add(new Column() { Type = typeof(string), Name = "Element Function" });
            columnData.Add(new Column() { Type = typeof(int), Name = "Object Number" });
            columnData.Add(new Column() { Type = typeof(string), Name = "Message" });

            var messages = new List<List<object>>()
            {
                new List<object>() { 1, 1, "Device", "836:36:53", "-", "-", 7004, "+Operating mode change rejected" },
                new List<object>() { 3, 2, "Device", "837:36:53", "-", "-", 15674, "+Configuration not released" },
                new List<object>() { 2, 3, "Elements", "837:38:53", "-", "-", 4007, "+User program stopped" },
                new List<object>() { 4, 4, "Product", "838:26:02", "-", "-", 7004, "+Password protection for device access is inactive" }
            };

            Initialize(new LogbookData() { ColumnData = columnData, RowData = messages });
        }
    }
}
