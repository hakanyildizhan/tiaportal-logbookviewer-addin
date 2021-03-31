using Sirius.LogbookViewer.Product;
using Sirius.LogbookViewer.UI.Service;
using System;
using System.IO;
using System.Linq;
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
        public string WindowTitle { get; set; } = "Import";

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
                if (_topmost == value)
                {
                    Topmost = value;
                    Topmost = !value;
                }
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
            Topmost = !System.Diagnostics.Debugger.IsAttached;
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
                Task loadingTask = null;

                try
                {
                    loadingTask = _waitIndicator?.ShowAsync("Logbook Import", "Importing logbook entries", $"Importing file '{fileName}'. Please wait.");
                    var waitTask = Task.Delay(TimeSpan.FromSeconds(1));
                    var data = await _parser.Parse(fileName);
                    await waitTask;
                    await loadingTask;
                    _waitIndicator?.ShowMessage("Operation complete");
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    if (data.RowData.Any())
                    {
                        GridIsInitialized = true;
                        Filter = new FilterViewModel();
                        Filter.Initialize(data);
                        Grid = new GridViewModel();
                        Grid.Initialize(data);
                    }

                    _waitIndicator?.Close();
                }
                catch (System.Exception ex)
                {
                    await loadingTask;

                    if (ex is IOException)
                    {
                        _waitIndicator?.Prompt("An error occured during import!", ex.Message, Prompt.Error);
                        return;
                    }

                    string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string errorFile = $"Sirius_LogbookViewer_errorlog_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss")}.txt";
                    string filePath = Path.Combine(appDataFolder, "Siemens", "SafetyLogbookViewerAddin", errorFile);
                    File.WriteAllText(filePath, ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException?.Message + "\r\n" + ex.InnerException?.StackTrace);
                    _waitIndicator?.Prompt("An error occured during import!", "See the error log for details: " + filePath, Prompt.Error);
                }
            });
        }
    }
}
