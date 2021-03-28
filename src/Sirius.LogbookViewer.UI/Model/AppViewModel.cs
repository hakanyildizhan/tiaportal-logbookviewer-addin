using Sirius.LogbookViewer.UI.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IFilePicker _filePicker => (IFilePicker)ServiceProvider.Resolve<IFilePicker>();
        private IWaitIndicator _waitIndicator => (IWaitIndicator)ServiceProvider.Resolve<IWaitIndicator>();
        private ObservableCollection<LogbookMessage> _messages;
        private IList<LogbookMessage> _allMessages;

        private bool _topmost;
        private WindowState _windowState;
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged(nameof(WindowState));
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

        public ObservableCollection<LogbookMessage> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public bool IsBusy { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }

        public ICommand ToggleErrorsCommand { get; set; }
        public ICommand ToggleWarningsCommand { get; set; }
        public ICommand TogglePrewarningsCommand { get; set; }
        public ICommand ToggleEventsCommand { get; set; }
        public ICommand ToggleOperatingErrorsCommand { get; set; }

        public ICommand SortCommand { get; set; }
        public bool? SortStatusSource { get; set; }
        public bool? SortStatusType { get; set; }
        public bool? SortStatusOperatingHours { get; set; }

        public AppViewModel()
        {
            _allMessages = new List<LogbookMessage>();
            Topmost = !System.Diagnostics.Debugger.IsAttached;
            ImportCommand = new RelayCommand(async () => await ImportAsync());
            CloseCommand = new RelayCommand(() => Close());
            MinimizeCommand = new RelayCommand(() => Minimize());

            ToggleErrorsCommand = new RelayParameterizedCommand((parameter) => ToggleMessageType(1, (bool)parameter));
            ToggleWarningsCommand = new RelayParameterizedCommand((parameter) => ToggleMessageType(3, (bool)parameter));
            TogglePrewarningsCommand = new RelayParameterizedCommand((parameter) => ToggleMessageType(4, (bool)parameter));
            ToggleOperatingErrorsCommand = new RelayParameterizedCommand((parameter) => ToggleMessageType(2, (bool)parameter));
            ToggleEventsCommand = new RelayParameterizedCommand((parameter) => ToggleMessageType(5, (bool)parameter));

            SortCommand = new RelayParameterizedCommand((parameter) => Sort((string)parameter));
        }

        private void Sort(string columnName)
        {
            columnName = "SortStatus" + columnName.Replace(" ", "");
            var sortFlagProperty = this.GetType().GetProperty(columnName);
            var sortFlag = sortFlagProperty.GetValue(this) as bool?;

            if (sortFlag == null)
            {
                sortFlagProperty.SetValue(this, true);
            }
            else if (sortFlag == true)
            {
                sortFlagProperty.SetValue(this, false);
            }
            else
            {
                sortFlagProperty.SetValue(this, null);
            }

            OnPropertyChanged(columnName);

            if (Messages == null || !Messages.Any())
            {
                return;
            }

            var activeSortFlagProperties = this.GetType().GetProperties().Where(p => p.Name.StartsWith("SortStatus") && (p.GetValue(this) as bool?).HasValue).ToList();

            var messages = Messages.OrderBy(m => m.Index);

            for (int i = 0; i < activeSortFlagProperties.Count; i++)
            {
                var propertyName = activeSortFlagProperties[i].Name.Replace("SortStatus", string.Empty);
                var propertyValue = activeSortFlagProperties[i].GetValue(this) as bool?;

                if (propertyValue == null)
                {
                    continue;
                }

                var logbookProperty = typeof(SafetyLogbookMessage).GetProperty(propertyName);

                if (i == 0)
                {
                    if (propertyValue == true)
                    {
                        messages = messages.OrderBy(m => logbookProperty.GetValue(m));
                    }
                    else
                    {
                        messages = messages.OrderByDescending(m => logbookProperty.GetValue(m));
                    }
                }
                else
                {
                    if (propertyValue == true)
                    {
                        messages = messages.ThenBy(m => logbookProperty.GetValue(m));
                    }
                    else
                    {
                        messages = messages.ThenByDescending(m => logbookProperty.GetValue(m));
                    }
                }
            }

            var updatedMessages = messages.ToList();

            for (int i = 0; i < updatedMessages.Count(); i++)
            {
                updatedMessages[i].Index = i + 1;
            }

            Messages = new ObservableCollection<LogbookMessage>(updatedMessages);
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

            var messageList = new List<LogbookMessage>();

            await RunCommandAsync(() => IsInBackground, false, async () =>
            {
                Task loadingTask = null;

                try
                {
                    loadingTask = _waitIndicator?.ShowAsync("Logbook Import", "Importing logbook entries", $"Importing file '{fileName}'. Please wait.");
                    var waitTask = Task.Delay(TimeSpan.FromSeconds(1));
                    var parser = await ParserFactory.GetParser(fileName);
                    var messages = parser.Parse();

                    foreach (var message in messages)
                    {
                        messageList.Add(message);
                    }

                    await waitTask;
                    await loadingTask;
                    _waitIndicator?.ShowMessage("Operation complete");
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    if (messageList.Any())
                    {
                        _allMessages = messageList;
                        Messages = new ObservableCollection<LogbookMessage>(messageList);
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

        private void ToggleMessageType(int messageType, bool show)
        {
            if (_allMessages == null || _allMessages.Count == 0)
            {
                return;
            }

            if (!show)
            {
                var newMessages = _messages.Where(m => ((SafetyLogbookMessage)m).Type != messageType).ToList();

                for (int i = 1; i <= newMessages.Count(); i++)
                {
                    newMessages[i - 1].Index = i;
                }

                Messages = new ObservableCollection<LogbookMessage>(newMessages);
            }
            else
            {
                var messagesToShow = _allMessages.Where(m => ((SafetyLogbookMessage)m).Type == messageType);
                messagesToShow.ToList().ForEach(m => Messages.Add(m));

                for (int i = 1; i <= Messages.Count(); i++)
                {
                    Messages[i - 1].Index = i;
                }

                OnPropertyChanged(nameof(Messages));
            }
        }
    }
}
