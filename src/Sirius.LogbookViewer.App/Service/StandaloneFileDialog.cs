using Sirius.LogbookViewer.UI.Service;
using Sirius.LogbookViewer.App.Model;

namespace Sirius.LogbookViewer.App.Service
{
    public class StandaloneFileDialog : IFilePicker
    {
        public string SelectFile()
        {
            string fileName = string.Empty;
            var dialog = new SelectFile();
            var model = new SelectFileViewModel(dialog);
            dialog.DataContext = model;

            if (dialog.ShowDialog() == true)
            {
                fileName = model.FilePath;
            }

            return fileName;
        }
    }
}
