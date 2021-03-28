using Microsoft.Win32;
using Sirius.LogbookViewer.UI.Service;

namespace Sirius.LogbookViewer.Service
{
    public class FilePicker : IFilePicker
    {
        public string SelectFile()
        {
            FileDialog dialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                DefaultExt = ".csv",
                Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Import",
            };

            string fileName = string.Empty;

            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }
    }
}
