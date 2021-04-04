using Microsoft.Win32;
using Sirius.LogbookViewer.UI.Service;
using System;
using System.Runtime.InteropServices;

namespace Sirius.LogbookViewer.App
{
    public class FilePicker : IFilePicker
    {
        public string SelectFile()
        {
            string fileName = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.RestoreDirectory = true;
            dialog.Multiselect = false;
            dialog.DefaultExt = ".csv";
            dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            dialog.Title = "Import";

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                return dialog.FileName;
            }

            return fileName;
        }
    }
}
