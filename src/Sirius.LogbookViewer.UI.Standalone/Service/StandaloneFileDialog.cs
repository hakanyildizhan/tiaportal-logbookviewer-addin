using Sirius.LogbookViewer.UI.Service;
using Sirius.LogbookViewer.UI.Standalone.Model;

namespace Sirius.LogbookViewer.UI.Standalone.Service
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
