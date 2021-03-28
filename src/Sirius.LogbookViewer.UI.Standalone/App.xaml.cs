using Sirius.LogbookViewer.UI.Model;
using Sirius.LogbookViewer.UI.Standalone.Service;
using System.Windows;

namespace Sirius.LogbookViewer.UI.Standalone
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
