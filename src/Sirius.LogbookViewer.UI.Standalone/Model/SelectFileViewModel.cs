using Sirius.LogbookViewer.UI.Model;
using System.Windows;
using System.Windows.Input;

namespace Sirius.LogbookViewer.UI.Standalone.Model
{
    public class SelectFileViewModel : BaseViewModel
    {
        private readonly Window _window;
        public string WindowTitle { get; set; } = "Select File";
        public string FilePath { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }

        public SelectFileViewModel(Window window)
        {
            _window = window;
            CloseCommand = new RelayCommand(() => Close());
            SelectFileCommand = new RelayCommand(() => SelectFile());
        }

        private void SelectFile()
        {
            _window.DialogResult = true;
            _window?.Close();
        }

        private void Close()
        {
            _window.DialogResult = false;
            _window?.Close();
        }
    }
}
