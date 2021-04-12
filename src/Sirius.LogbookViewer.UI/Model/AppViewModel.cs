using Sirius.LogbookViewer.Product;
using Sirius.LogbookViewer.UI.Service;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sirius.LogbookViewer.UI.Model
{
    /// <summary>
    /// Main application data context.
    /// </summary>
    public class AppViewModel : BaseViewModel
    {
        private IFilePicker _filePicker => (IFilePicker)ServiceProvider?.Resolve<IFilePicker>();
        private IWaitIndicator _waitIndicator => (IWaitIndicator)ServiceProvider?.Resolve<IWaitIndicator>();
        private IParser _parser => (IParser)ServiceProvider?.Resolve<IParser>();
        private UIResourceManager _uiResourceMgr => (UIResourceManager)ServiceProvider?.Resolve<UIResourceManager>();

        private GridViewModel _grid;
        private FilterViewModel _filter;
        private bool _topmost;
        private WindowState _windowState;
        private bool _gridIsInitialized;

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged(nameof(WindowState));
            }
        }

        public bool GridIsInitialized
        {
            get => _gridIsInitialized;
            set
            {
                _gridIsInitialized = value;
                OnPropertyChanged(nameof(GridIsInitialized));
            }
        }

        /// <summary>
        /// Title of the window to show at the top.
        /// </summary>
        public string WindowTitle => _uiResourceMgr.GetString("AppTitle");

        /// <summary>
        /// Import button text.
        /// </summary>
        public string ImportButtonContent => _uiResourceMgr.GetString("Import");

        /// <summary>
        /// Close button text.
        /// </summary>
        public string CloseButtonContent => _uiResourceMgr.GetString("Close");

        /// <summary>
        /// Whether this window will be displayed at the top of all other windows.
        /// </summary>
        public bool Topmost
        {
            get => _topmost;
            set
            {
                _topmost = value;
                OnPropertyChanged(nameof(Topmost));
            }
        }

        public bool IsInBackground
        {
            get => !_topmost;
            set
            {
#if RELEASE
                if (_topmost == value)
                {
                    Topmost = value;
                    Topmost = !value;
                }
#endif
            }
        }

        public GridViewModel Grid 
        {
            get => _grid;
            set
            {
                _grid = value;
                OnPropertyChanged(nameof(Grid));
            }
        }

        public FilterViewModel Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged(nameof(Filter));
            }
        }

        public bool IsBusy { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }

        public AppViewModel()
        {
#if RELEASE
            Topmost = true;
#endif
            ImportCommand = new RelayCommand(async () => await ImportAsync());
            CloseCommand = new RelayCommand(() => Close());
            MinimizeCommand = new RelayCommand(() => Minimize());
        }

        private void Minimize()
        {
            WindowState = WindowState.Minimized;
        }

        private void Close()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private async Task ImportAsync()
        {
            string fileName = _filePicker.SelectFile();

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            await RunCommandAsync(() => IsInBackground, false, async () =>
            {
                // display loading message
                await Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await _waitIndicator?.ShowAsync(_uiResourceMgr.GetString("ImportPopupTitle"), _uiResourceMgr.GetString("ImportPopupHeader"), $"{_uiResourceMgr.GetString("ImportPopupMessage")} '{fileName}'. {_uiResourceMgr.GetString("ImportPopupWaitMessage")}");
                }), System.Windows.Threading.DispatcherPriority.Send);

                try
                {
                    LogbookData data = null;
                    var loadData = _parser.Parse(fileName);

                    // parse data & initialize grid data
                    await Task.Run(async () =>
                    {
                        Grid = new GridViewModel();
                        data = await loadData;
                        Grid.InitializeData(data);
                    });

                    // initialize UI
                    Exception uiException = null;
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            Filter = new FilterViewModel();
                            Filter.Initialize(data);
                            GridIsInitialized = true;
                            Grid.InitializeUI(data);
                        }
                        catch (Exception ex)
                        {
                            uiException = ex;
                        }
                    }), System.Windows.Threading.DispatcherPriority.SystemIdle);

                    if (uiException != null)
                    {
                        throw uiException;
                    }

                    _waitIndicator?.ShowMessage(_uiResourceMgr.GetString("ImportPopupSuccessMessage"));
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    _waitIndicator?.Close();
                }
                catch (System.Exception ex)
                {
                    if (ex is IOException)
                    {
                        _waitIndicator?.Prompt(_uiResourceMgr.GetString("ImportPopupErrorMessage"), ex.Message, Prompt.Error);
                        return;
                    }

                    string errorFile = $"apperror_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss")}.log";
                    string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Siemens AG", "Logbook Viewer AddIn");
                    Directory.CreateDirectory(appFolder);
                    string filePath = Path.Combine(appFolder, errorFile);
                    File.WriteAllText(filePath, ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException?.Message + "\r\n" + ex.InnerException?.StackTrace);
                    _waitIndicator?.Prompt(_uiResourceMgr.GetString("ImportPopupErrorMessage"), $"{_uiResourceMgr.GetString("ImportPopupDetails")} {filePath}", Prompt.Error);
                }
            });
        }
    }
}
