using Sirius.LogbookViewer.Product;
using Sirius.LogbookViewer.UI.Service;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Sirius.LogbookViewer.UI.Model
{
    public class FilterViewModel : BaseViewModel
    {
        private GroupBox _groupBox;
        private bool _isVisible;
        private IResourceManager _resourceManager => (IResourceManager)ServiceProvider?.Resolve<IResourceManager>();
        private UIResourceManager _uiResourceMgr => (UIResourceManager)ServiceProvider?.Resolve<UIResourceManager>();

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public GroupBox FilterSection
        {
            get => _groupBox;
            set
            {
                _groupBox = value;
                OnPropertyChanged(nameof(FilterSection));
            }
        }

        public void Initialize(LogbookData data)
        {
            if (!data.ColumnData.Any(c => c.Filter))
            {
                IsVisible = false;
                return;
            }

            var groupBox = new GroupBox();
            groupBox.Header = _uiResourceMgr.GetString("DisplayedLogbooks");

            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            var filterData = data.ColumnData.First(c => c.Filter).FilterData;
            int totalFilterContentLength = 0;

            foreach (var keyValuePair in filterData)
            {
                string columnValue = keyValuePair.Key;
                var columnFilterData = filterData[columnValue];

                var filterStackPanel = new StackPanel();

                if (!string.IsNullOrEmpty(columnFilterData.IconPath))
                {
                    var imageContentControl = new ContentControl();
                    imageContentControl.Margin = new Thickness(-3, -1, 0, 2);
                    imageContentControl.VerticalAlignment = VerticalAlignment.Center;

                    var filterImage = new Image();
                    filterImage.Width = 30;
                    filterImage.Source = new BitmapImage(new Uri($"pack://application:,,,/{_resourceManager.GetType().Assembly.GetName().Name};component/{columnFilterData.IconPath}"));

                    imageContentControl.Content = filterImage;
                    filterStackPanel.Children.Add(imageContentControl);
                }

                var checkbox = new CheckBox();
                checkbox.Content = _resourceManager.GetString(columnFilterData.DisplayValue);
                totalFilterContentLength += checkbox.Content.ToString().Length;
                checkbox.VerticalAlignment = VerticalAlignment.Center;
                checkbox.IsChecked = true;
                BindingOperations.SetBinding(checkbox, Button.CommandProperty, new Binding("Grid.FilterCommand"));
                
                var commandParameterBinding = new MultiBinding();
                commandParameterBinding.Converter = new FilterCommandParameterConverter();
                var isCheckedBinding = new Binding("IsChecked");
                isCheckedBinding.RelativeSource = RelativeSource.Self;
                commandParameterBinding.Bindings.Add(isCheckedBinding);
                var contentBinding = new Binding("Content");
                contentBinding.RelativeSource = RelativeSource.Self;
                commandParameterBinding.Bindings.Add(contentBinding);
                BindingOperations.SetBinding(checkbox, Button.CommandParameterProperty, commandParameterBinding);

                filterStackPanel.Children.Add(checkbox);
                stackPanel.Children.Add(filterStackPanel);
            }

            var stackPanelStyle = new Style();
            stackPanelStyle.TargetType = typeof(StackPanel);

            // set spacing between filter checkboxes
            int checkboxRightMargin = 0;

            if (totalFilterContentLength < 50)
            {
                checkboxRightMargin = 64;
            }
            else if (totalFilterContentLength >= 50 && totalFilterContentLength < 60)
            {
                checkboxRightMargin = totalFilterContentLength;
            }
            else
            {
                checkboxRightMargin = 120 - totalFilterContentLength;
            }

            stackPanelStyle.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(5, 0, checkboxRightMargin, 0)));
            stackPanelStyle.Setters.Add(new Setter(StackPanel.OrientationProperty, Orientation.Horizontal));
            stackPanel.Resources[typeof(StackPanel)] = stackPanelStyle;

            groupBox.Content = stackPanel;
            FilterSection = groupBox;
            IsVisible = true;
        }
    }
}
