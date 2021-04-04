using Sirius.LogbookViewer.UI.Model;
using Sirius.LogbookViewer.App.Service;
using System.Windows;
using System.Globalization;
using System.Threading;

namespace Sirius.LogbookViewer.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CultureInfo appCulture = new CultureInfo("en-US");

            if (e.Args.Length == 2 || e.Args[0] == "--culture")
            {
                try
                {
                    appCulture = new CultureInfo(e.Args[1]);
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Incorrect parameter.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Current.Shutdown(1);
                }
            }

            Thread.CurrentThread.CurrentCulture = appCulture;
            Thread.CurrentThread.CurrentUICulture = appCulture;
            AppViewModel.ServiceProvider = ServiceContainer.Instance;
        }
    }
}
