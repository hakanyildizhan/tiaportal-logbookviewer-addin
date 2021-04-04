using Sirius.LogbookViewer.UI.Model;
using Sirius.LogbookViewer.App.Service;
using System.Windows;

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
            AppViewModel.ServiceProvider = ServiceContainer.Instance;
        }
    }
}
