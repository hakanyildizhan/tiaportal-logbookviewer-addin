using Sirius.LogbookViewer.UI.Model;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Sirius.LogbookViewer.UI.Service
{
    public class LoadingDialog : BaseViewModel, IWaitIndicator
    {
        private static LoadingWindow _window;

        public void Show()
        {
            var model = new LoadingWindowViewModel();
            Task.Run(async () =>
            {
                string animationFile = await GetAnimationFilePath();
                model.LoadingAnimationSource = !string.IsNullOrEmpty(animationFile) ? new Uri(animationFile) : null;
                model.CanAnimate = !string.IsNullOrEmpty(animationFile);
            });
            _window = new LoadingWindow();
            _window.DataContext = model;
            _window.Show();
            _window.Activate();
        }

        public void Show(string message)
        {
            var model = new LoadingWindowViewModel();
            model.Message = message;
            Task.Run(async () => 
            { 
                string animationFile = await GetAnimationFilePath();
                model.LoadingAnimationSource = !string.IsNullOrEmpty(animationFile) ? new Uri(animationFile) : null;
                model.CanAnimate = !string.IsNullOrEmpty(animationFile);
            });
            _window = new LoadingWindow();
            _window.DataContext = model;
            _window.Show();
            _window.Activate();
        }

        public async Task ShowAsync(string message)
        {
            var model = new LoadingWindowViewModel();
            var getAnimationTask = GetAnimationFilePath();
            Close();
            _window = new LoadingWindow();
            _window.DataContext = model;
            _window.Show();
            _window.Activate();
            string animationFile = await getAnimationTask;

            if (!string.IsNullOrEmpty(animationFile))
            {
                (_window.DataContext as LoadingWindowViewModel).LoadingAnimationSource = new Uri(animationFile);
                (_window.DataContext as LoadingWindowViewModel).CanAnimate = true;
            }
        }

        public async Task ShowAsync(string windowTitle, string header, string message)
        {
            var model = new LoadingWindowViewModel();
            var getAnimationTask = GetAnimationFilePath();
            model.Message = message;
            model.MessageTitle = header;
            model.WindowTitle = windowTitle;
            _window = new LoadingWindow();
            _window.DataContext = model;
            _window.Show();
            _window.Activate();

            string animationFile = await getAnimationTask;

            if (!string.IsNullOrEmpty(animationFile))
            {
                (_window.DataContext as LoadingWindowViewModel).LoadingAnimationSource = new Uri(animationFile);
                (_window.DataContext as LoadingWindowViewModel).CanAnimate = true;
            }
        }

        public void ShowMessage(string message)
        {
            if (_window != null)
            {
                (_window.DataContext as LoadingWindowViewModel).Message = message;
            }
        }

        public void Prompt(string message)
        {
            Prompt(message, Service.Prompt.None);
        }

        public void Prompt(string message, Prompt promptType)
        {
            Prompt(string.Empty, message, promptType);
        }

        public void Prompt(string header, string message, Prompt promptType)
        {
            if (_window == null)
            {
                return;
            }

            (_window.DataContext as LoadingWindowViewModel).AcknowledgeCommand = new RelayCommand(() => Close());
            (_window.DataContext as LoadingWindowViewModel).Message = message;

            if (!string.IsNullOrEmpty(header))
            {
                (_window.DataContext as LoadingWindowViewModel).MessageTitle = header;
            }

            (_window.DataContext as LoadingWindowViewModel).Prompt = true;
            (_window.DataContext as LoadingWindowViewModel).CanAnimate = false;
            (_window.DataContext as LoadingWindowViewModel).ImageSource = GetImageSourceForPrompt(promptType);
        }

        public void Close()
        {
            _window?.Close();
        }

        private Uri GetImageSourceForPrompt(Prompt prompt)
        {
            object resource = null;

            switch (prompt)
            {
                case Service.Prompt.Error:
                    resource = Application.Current.Resources["PromptError"];
                    break;
                case Service.Prompt.None:
                default:
                    resource = Application.Current.Resources["HourglassImage"];
                    break;
            }

            return ((BitmapImage)resource).UriSource;
        }

        private async Task<string> GetAnimationFilePath()
        {
            return await Task.Run(() => {
                try
                {
                    string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string filePath = Path.Combine(appDataFolder, "Siemens", "SafetyLogbookViewerAddin", "Hourglass.gif");

                    if (!File.Exists(filePath))
                    {
                        var siemensFolder = Path.Combine(appDataFolder, "Siemens");

                        if (!Directory.Exists(siemensFolder))
                        {
                            Directory.CreateDirectory(siemensFolder);
                        }

                        string addinFolder = Path.Combine(siemensFolder, "SafetyLogbookViewerAddin");

                        if (!Directory.Exists(addinFolder))
                        {
                            Directory.CreateDirectory(addinFolder);
                        }

                        var assembly = Assembly.GetAssembly(typeof(MainWindow));
                        var resourceName = "Sirius.LogbookViewer.UI.Resource.Animation.Hourglass.gif";

                        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                        using (Stream s = File.Create(filePath))
                        {
                            stream.CopyTo(s);
                        }
                    }

                    return filePath;
                }
                catch (Exception) // file could not be created. return null to show still image
                {
                    return null;
                }
            });
        }
    }
}
