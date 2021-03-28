using Sirius.LogbookViewer.Service;
using Sirius.LogbookViewer.UI.Model;
using System;
using System.Reflection;
using System.Windows;

namespace Sirius.LogbookViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string STYLES_URI = "pack://application:,,,/Sirius.LogbookViewer.UI;component/Style/";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            System.Windows.Application.ResourceAssembly = Assembly.GetAssembly(typeof(Sirius.LogbookViewer.UI.MainWindow));

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri("pack://application:,,,/Sirius.LogbookViewer;component/Style/AddinStyle.xaml") });

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Text.xaml") });

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Color.xaml") });

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Icon.xaml") });

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Window.xaml") });

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Checkbox.xaml") });

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Grid.xaml") });
            
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Panel.xaml") });
            
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "GroupBox.xaml") });
            
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(STYLES_URI + "Button.xaml") });

            StartupUri = new Uri("pack://application:,,,/Sirius.LogbookViewer.UI;component/MainWindow.xaml");
            AppViewModel.ServiceProvider = ServiceContainer.Instance;
        }
    }
}
