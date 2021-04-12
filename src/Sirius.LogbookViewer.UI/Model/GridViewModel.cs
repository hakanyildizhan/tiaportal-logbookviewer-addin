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
using System.ComponentModel;

namespace Sirius.LogbookViewer.UI.Model
{
    public class GridViewModel : BaseViewModel
    {
        private ObservableCollection<ExpandoObject> _messages;
        private ListView _gridContent;
        private IResourceManager _resourceManager => (IResourceManager)ServiceProvider?.Resolve<IResourceManager>();

        protected List<Column> ColumnData { get; set; }
        protected List<ExpandoObject> AllMessages { get; set; }

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
        public ICommand FilterCommand { get; set; }

        public GridViewModel()
        {
            SortCommand = new RelayParameterizedCommand((parameter) => Sort((string)parameter));
            FilterCommand = new RelayParameterizedCommand((parameter) => Toggle((FilterCommandParameter)parameter));
            SortStatus = new Dictionary<string, bool?>();
        }

        public void InitializeData(LogbookData data)
        {
            ColumnData = data.ColumnData;

            List<ExpandoObject> messages = new List<ExpandoObject>();
            for (int i = 0; i < data.RowData.Count; i++)
            {
                var logbookMessage = new ExpandoObject();
                for (int j = 0; j < ColumnData.Count; j++)
                {
                    object columnData = null;

                    try
                    {
                        var converter = TypeDescriptor.GetConverter(ColumnData[j].Type);
                        columnData = converter.ConvertTo(data.RowData[i][j], ColumnData[j].Type);
                    }
                    catch
                    {
                        if (!string.IsNullOrEmpty(data.RowData[i][j].ToString()))
                        {
                            columnData = data.RowData[i][j];
                        }
                        else if (!string.IsNullOrEmpty(ColumnData[j].Placeholder))
                        {
                            columnData = ColumnData[j].Placeholder;
                        }
                        else
                        {
                            columnData = Activator.CreateInstance(ColumnData[j].Type);
                        }
                    }

                    (logbookMessage as IDictionary<string, Object>).Add(ColumnData[j].Name.Replace(" ", ""), columnData);
                }
                messages.Add(logbookMessage);
            }

            Messages = new ObservableCollection<ExpandoObject>(messages);
            AllMessages = messages;
        }

        public void InitializeUI(LogbookData data)
        {
            var gridView = new GridView();

            for (int i = 0; i < ColumnData.Count; i++)
            {
                Column column = ColumnData[i];
                var gridViewColumn = new GridViewColumn();
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
                    imageBinding.ConverterParameter = new DataToIconConverterParameter() { IconData = column.IconData, AssemblyName = _resourceManager != null ? _resourceManager.GetType().Assembly.GetName().Name : string.Empty };
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
                columnHeader.Content = !DesignerProperties.GetIsInDesignMode(columnHeader) ? _resourceManager.GetString(ResourceType.UI, column.Name) : column.Name;

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

                // autosize column
                if (double.IsNaN(gridViewColumn.Width))
                {
                    gridViewColumn.Width = gridViewColumn.ActualWidth;
                }
                gridViewColumn.Width = double.NaN;

                // add column
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

            var updatedMessages = activeSortColumns.Any() ? messages.ToList() : AllMessages.ToList();

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

        private void Toggle(FilterCommandParameter arguments)
        {
            if (AllMessages == null || AllMessages.Count == 0)
            {
                return;
            }

            string columnDisplayValue = arguments.ColumnValue; // Error
            string columnValue = ColumnData.First(c => c.Filter).FilterData.First(d => d.Value.DisplayValue.Equals(columnDisplayValue)).Key; // "1"
            string filterColumn = ColumnData.First(c => c.Filter).Name.Replace(" ", ""); // Type
            string indexColumnName = ColumnData.First(c => c.IsIndex).Name.Replace(" ", ""); // Index

            if (!arguments.Selected)
            {
                var newMessages = _messages.Where(m => ((IDictionary<string, object>)m)[filterColumn].ToString() != columnValue).ToList();

                if (ColumnData.Any(c => c.IsIndex))
                {
                    for (int i = 1; i <= newMessages.Count(); i++)
                    {
                        ((IDictionary<string, object>)newMessages[i - 1])[indexColumnName] = i;
                    }
                }

                Messages.Clear();
                newMessages.ForEach(m => Messages.Add(m));
            }
            else
            {
                var messagesToShow = AllMessages.Where(m => ((IDictionary<string, object>)m)[filterColumn].ToString() == columnValue);
                messagesToShow.ToList().ForEach(m => Messages.Add(m));

                for (int i = 1; i <= Messages.Count(); i++)
                {
                    ((IDictionary<string, object>)Messages[i - 1])[indexColumnName] = i;
                }

                OnPropertyChanged(nameof(Messages));
            }
        }
    }
}
