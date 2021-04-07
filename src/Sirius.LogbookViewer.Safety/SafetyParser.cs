using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sirius.LogbookViewer.Product;

namespace Sirius.LogbookViewer.Safety
{
    /// <summary>
    /// Safety specific parser for logbook exports.
    /// </summary>
    [Export(typeof(IParser))]
    public class SafetyParser : IParser
    {
        private readonly SafetyResourceManager _resManager;

        public SafetyParser()
        {
            _resManager = new SafetyResourceManager();
        }

        public async Task<LogbookData> Parse(string filePath)
        {
            var fileMeta = await GetFileMeta(filePath);
            var rows = new List<List<object>>();
            var columnData = new List<Column>();
            columnData.Add(new Column() 
            { 
                Type = typeof(int), 
                Name = _resManager.GetString("Index"), 
                IsIndex = true 
            });
            columnData.Add(new Column() 
            { 
                Type = typeof(int), 
                Name = _resManager.GetString("Type"), 
                Filter = true,
                FilterData = new Dictionary<string, FilterOption>()
                {
                    { "1", new FilterOption() { DisplayValue = _resManager.GetString("Error"), IconPath = "Resources/Icon/error.png" } },
                    { "2", new FilterOption() { DisplayValue = _resManager.GetString("Operating Error"), IconPath = "Resources/Icon/fault.png" } },
                    { "3", new FilterOption() { DisplayValue = _resManager.GetString("Warning"), IconPath = "Resources/Icon/trip.png" } },
                    { "4", new FilterOption() { DisplayValue = _resManager.GetString("Prewarning"), IconPath = "Resources/Icon/prewarn.png" } },
                    { "5", new FilterOption() { DisplayValue = _resManager.GetString("Event"), IconPath = "Resources/Icon/event.png" } }
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
            columnData.Add(new Column() 
            { 
                Type = typeof(string),    
                Name = _resManager.GetString("Source"), Sortable = true 
            });
            columnData.Add(new Column() 
            { 
                Type = typeof(string),    
                Name = _resManager.GetString("Operating Hours"), 
                Sortable = true 
            });
            columnData.Add(new Column() 
            { 
                Type = typeof(int),       
                Name = _resManager.GetString("Element Number"),
                Placeholder = "-"
            });
            columnData.Add(new Column() 
            { 
                Type = typeof(string),    
                Name = _resManager.GetString("Element Function"),
                Placeholder = "-"
            });
            columnData.Add(new Column() 
            { 
                Type = typeof(int),      
                Name = _resManager.GetString("Object Number") 
            });
            columnData.Add(new Column() 
            { 
                Type = typeof(string),    
                Name = _resManager.GetString("Message") 
            });

            foreach (var parts in fileMeta.Rows)
            {
                var row = new List<object>();

                // column 0 - index
                bool valid = int.TryParse(parts[0], out int messageIndex);

                if (!valid)
                {
                    continue;
                }

                row.Add(messageIndex);

                // column 1 - message type (ES) OR type ID (Classic)
                bool isNumber = int.TryParse(parts[1], out int messageType);

                if (!isNumber)
                {
                    row.Add(GetMessageTypeCode(parts[1].Trim()));
                }
                else
                {
                    row.Add(messageType);
                }
                
                // column 2 - source number (Classic) OR source (ES)
                isNumber = int.TryParse(parts[2], out int sourceNumber);

                if (!isNumber)
                {
                    string sourceInCulture = _resManager.GetString(ResourceType.Message, parts[2].Trim());
                    row.Add(sourceInCulture);
                }
                else
                {
                    row.Add(GetSource(sourceNumber));
                }

                // column 3 - operating hours
                row.Add(parts[3].Trim());

                // column 4 - element number
                if (string.IsNullOrEmpty(parts[4]))
                {
                    row.Add("-");
                }
                else
                {
                    isNumber = int.TryParse(parts[4].Trim(), out int elementNumber);

                    if (isNumber)
                    {
                        row.Add(elementNumber);
                    }
                    else
                    {
                        row.Add("-");
                    }
                }

                // column 5 - element function
                row.Add(string.IsNullOrEmpty(parts[5]) ? "-" : parts[5].Trim());

                // column 6 - object number
                valid = int.TryParse(parts[6], out int objectNumber);

                if (!valid)
                {
                    continue;
                }

                row.Add(objectNumber);

                // column 7 - message
                string message = "-";
                
                if (!string.IsNullOrEmpty(parts[6]) && !string.IsNullOrEmpty(parts[7]))
                {
                    string messageToSearch = parts[7].Trim().TrimStart(new char[] { '+', '-' }).Trim();
                    string messageInCulture = _resManager.GetString(ResourceType.Message, messageToSearch);
                    message = !string.IsNullOrEmpty(messageInCulture) ? parts[7].Trim().Replace(messageToSearch, messageInCulture) : parts[7].Trim();
                }
                else if (!string.IsNullOrEmpty(parts[7]))
                {
                    message = parts[7].Trim();
                }

                row.Add(message);

                rows.Add(row);
            }

            return new LogbookData() { ColumnData = columnData, RowData = rows };
        }

        private async Task<FileMeta> GetFileMeta(string filePath)
        {
            var columns = new List<string>();
            var rows = new List<List<string>>();
            bool atColumnRow = false;
            int numColumns = 0;

            var fileData = await FileParser.GetDataAsync(filePath);

            foreach (string line in fileData)
            {
                if (columns.Count == 0)
                {
                    if (!atColumnRow)
                    {
                        atColumnRow = line.Contains(',') && line.Split(',').Where(c => !string.IsNullOrEmpty(c)).Count() > 1;

                        if (atColumnRow)
                        {
                            var possibleColumns = line.Split(',').Where(c => !string.IsNullOrEmpty(c));
                            numColumns = possibleColumns.Count();
                            columns = possibleColumns.ToList();
                        }

                        continue;
                    }
                }
                else if (rows.Count == 0)
                {
                    if (!line.Contains(','))
                    {
                        atColumnRow = false;
                        numColumns = 0;
                        columns.Clear();
                    }
                    else
                    {
                        var rowData = line.Split(',').Where(c => !string.IsNullOrEmpty(c));
                        int numData = rowData.Count();

                        if (numData == numColumns)
                        {
                            rows.Add(rowData.ToList());
                        }
                        else
                        {
                            atColumnRow = false;
                            numColumns = 0;
                            columns.Clear();
                        }
                    }
                }
                else
                {
                    var rowData = line.Split(',').Where(c => !string.IsNullOrEmpty(c));
                    rows.Add(rowData.ToList());
                }
            }

            if (columns.Count == 0)
            {
                throw new Exception("File is corrupt.");
            }

            return new FileMeta()
            {
                Columns = columns,
                Rows = rows
            };
        }

        private string GetSource(int sourceNumber)
        {
            switch (sourceNumber)
            {
                case 1: return _resManager.GetString("Device");
                case 2: return _resManager.GetString("Elements");
                case 3: return _resManager.GetString("Communication");
                case 4: return _resManager.GetString("Product-specific");
                default: return "Unknown";
            }
        }

        private int GetMessageTypeCode(string messageType)
        {
            messageType = _resManager.GetString(ResourceType.UI, messageType, new System.Globalization.CultureInfo("en-US"));

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
