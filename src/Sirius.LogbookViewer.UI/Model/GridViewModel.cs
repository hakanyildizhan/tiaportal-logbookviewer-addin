using Sirius.LogbookViewer.Product;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Dynamic;
using System;

namespace Sirius.LogbookViewer.UI.Model
{
    public class GridViewModel : BaseViewModel
    {
        private ObservableCollection<ExpandoObject> _messages;
        private ListView _gridContent;
        private IResourceManager _resourceManager => (IResourceManager)ServiceProvider?.Resolve<IResourceManager>();

        protected List<Column> ColumnData { get; set; }
        protected List<List<object>> AllMessages { get; set; }

        public ObservableCollection<ExpandoObject> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public ListView GridContent
        {
            get => _gridContent;
            set
            {
                _gridContent = value;
                OnPropertyChanged(nameof(GridContent));
            }
        }

        public Dictionary<string, bool?> SortStatus { get; set; }

        public ICommand SortCommand { get; set; }

        public GridViewModel()
        {
            SortCommand = new RelayParameterizedCommand((parameter) => Sort((string)parameter));
            SortStatus = new Dictionary<string, bool?>();
        }

        public void Initialize(LogbookData data)
        {
            ColumnData = data.ColumnData;
            AllMessages = data.RowData;

            List<ExpandoObject> messages = new List<ExpandoObject>();
            for (int i = 0; i < data.RowData.Count; i++)
            {
                var logbookMessage = new ExpandoObject();
                for (int j = 0; j < ColumnData.Count; j++)
                {
                    (logbookMessage as IDictionary<string, Object>).Add(ColumnData[j].Name.Replace(" ", ""), data.RowData[i][j]);
                }
                messages.Add(logbookMessage);
            }

            Messages = new ObservableCollection<ExpandoObject>(messages);
            var gridView = new GridView();

            for (int i = 0; i < ColumnData.Count; i++)
            {
                Column column = ColumnData[i];
                var gridViewColumn = new GridViewColumn();
                gridViewColumn.Width = i != ColumnData.Count - 1 ? column.Name.Length * 10 : column.Name.Length * 20;
                string columnNameShort = column.Name.Replace(" ", "");

                // create datatemplate contents
                if (column.UseIcon)
                {
                    var contentControlFactory = new FrameworkElementFactory(typeof(ContentControl));
                    contentControlFactory.SetValue(FrameworkElement.HeightProperty, 25d);
                    contentControlFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, -4, -8, -4));

                    var imageFactory = new FrameworkElementFactory(typeof(Image));
                    imageFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    imageFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

                    Binding imageBinding = new Binding(columnNameShort);
                    imageBinding.Converter = new DataToIconConverter();
                    imageBinding.ConverterParameter = new DataToIconConverterParameter() { IconData = column.IconData, AssemblyName = _resourceManager != null ? _resourceManager.GetType().Assembly.GetName().Name : "Sirius.LogbookViewer.Safety" };
                    imageFactory.SetBinding(Image.SourceProperty, imageBinding);

                    contentControlFactory.AppendChild(imageFactory);
                    gridViewColumn.CellTemplate = new DataTemplate() { VisualTree = contentControlFactory };
                }
                else
                {
                    var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(4, 0, 0, 0));
                    Binding textBinding = new Binding(columnNameShort);
                    textBlockFactory.SetBinding(TextBlock.TextProperty, textBinding);
                    gridViewColumn.CellTemplate = new DataTemplate() { VisualTree = textBlockFactory };
                }

                var columnHeader = new GridViewColumnHeader();
                columnHeader.Content = column.Name;

                if (column.Sortable)
                {
                    SortStatus.Add(columnNameShort, null);

                    BindingOperations.SetBinding(columnHeader, SortProperty.ValueProperty, new Binding($"Grid.SortStatus[{columnNameShort}]"));
                    BindingOperations.SetBinding(columnHeader, Button.CommandProperty, new Binding("Grid.SortCommand"));

                    Binding commandParameterBinding = new Binding("Content");
                    commandParameterBinding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
                    BindingOperations.SetBinding(columnHeader, Button.CommandParameterProperty, commandParameterBinding);
                }
                else
                {
                    BindingOperations.SetBinding(columnHeader, SortProperty.ValueProperty, new Binding(null));
                }

                gridViewColumn.Header = columnHeader;
                gridView.Columns.Add(gridViewColumn);
            }

            var itemsContainerStyle = new Style();
            itemsContainerStyle.TargetType = typeof(ListViewItem);
            itemsContainerStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, 20d));

            GridContent = new ListView()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ItemsSource = Messages,
                ItemContainerStyle = itemsContainerStyle,
                View = gridView
            };
        }

        private void Sort(string columnName)
        {
            columnName = columnName.Replace(" ", "");
            var sortFlag = SortStatus[columnName];

            if (sortFlag == null)
            {
                SortStatus[columnName] = true;
            }
            else if (sortFlag == true)
            {
                SortStatus[columnName] = false;
            }
            else
            {
                SortStatus[columnName] = null;
            }

            OnPropertyChanged(nameof(SortStatus));

            if (Messages == null || !Messages.Any())
            {
                return;
            }

            List<string> activeSortColumns = SortStatus.Where(s => s.Value == true || s.Value == false).Select(s => s.Key).ToList();
            var messages = ColumnData.Any(c => c.IsIndex) ? Messages.OrderBy(m => ((IDictionary<string, object>)m)[ColumnData[ColumnData.FindIndex(c => c.IsIndex)].Name]) : Messages.OrderBy(m => ((IDictionary<string, object>)m).First().Key);

            for (int i = 0; i < activeSortColumns.Count; i++)
            {
                string activeSortColumn = activeSortColumns[i];

                if (i == 0)
                {
                    if (SortStatus[activeSortColumns[i]] == true)
                    {
                        messages = messages.OrderBy(m => ((IDictionary<string, object>)m)[activeSortColumn]);
                    }
                    else
                    {
                        messages = messages.OrderByDescending(m => ((IDictionary<string, object>)m)[activeSortColumn]);
                    }
                }
                else
                {
                    if (SortStatus[activeSortColumns[i]] == true)
                    {
                        messages = messages.ThenBy(m => ((IDictionary<string, object>)m)[activeSortColumn]);
                    }
                    else
                    {
                        messages = messages.ThenByDescending(m => ((IDictionary<string, object>)m)[activeSortColumn]);
                    }
                }
            }

            var updatedMessages = messages.ToList();

            if (ColumnData.Any(c => c.IsIndex))
            {
                for (int i = 0; i < updatedMessages.Count(); i++)
                {
                    ((IDictionary<string, object>)updatedMessages[i])[ColumnData.First(c => c.IsIndex).Name] = i + 1;
                }
            }

            Messages.Clear();
            updatedMessages.ForEach(m => Messages.Add(m));
        }
    }
}
