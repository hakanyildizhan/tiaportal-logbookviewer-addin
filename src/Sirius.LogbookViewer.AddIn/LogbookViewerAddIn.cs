using System;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows;

namespace Sirius.LogbookViewer.AddIn
{
    public class LogbookViewerAddIn : ContextMenuAddIn
    {
        private readonly TiaPortal _tiaPortal;

        public LogbookViewerAddIn(TiaPortal tiaPortal) : base("Logbook Viewer")
        {
            _tiaPortal = tiaPortal;
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Offline logbook viewer...", OnClick);
        }

        private void OnClick(MenuSelectionProvider menuSelectionProvider)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Siemens AG", "Logbook Viewer AddIn");

            try
            {
                using (_tiaPortal.ExclusiveAccess("Logbook Viewer is being loaded, please wait..."))
                {
                    if (IsDeployNecessary(folder))
                    {
                        Directory.Delete(folder, true);
                        Directory.CreateDirectory(folder);
                        string packagePath = Path.Combine(folder, "Package.zip");

                        using (Stream stream = GetResource("Package.zip"))
                        using (FileStream fileStream = new FileStream(packagePath, FileMode.CreateNew))
                        {
                            for (int i = 0; i < stream.Length; i++)
                            {
                                fileStream.WriteByte((byte)stream.ReadByte());
                            }

                            fileStream.Close();
                        }

                        ZipFile.ExtractToDirectory(packagePath, folder);
                        File.Delete(packagePath);
                    }

                    CultureInfo currentCulture = null;

                    try
                    {
                        currentCulture = (CultureInfo)_tiaPortal.SettingsFolders[0].Settings.First(s => s.Name.Equals("UserInterfaceLanguage")).Value;
                    }
                    catch (Exception)
                    {
                        currentCulture = new CultureInfo("en-US");
                    }

                    Siemens.Engineering.AddIn.Utilities.Process.Start(Path.Combine(folder, "Sirius.LogbookViewer.App.exe"), $"--culture {currentCulture.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while initializing the add-in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Directory.CreateDirectory(folder);
                File.AppendAllText(Path.Combine(folder, "error.log"), $"{ex.Message}\r\n{ex.StackTrace}\r\n\r\n");
            }
        }

        private bool IsDeployNecessary(string path)
        {
            if (!Directory.Exists(path))
            {
                return true;
            }

            var existingFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            using (Stream stream = GetResource("Package.manifest"))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    string[] fileManifest = line.Split('|');
                    string fileRelativePath = fileManifest[0];
                    string existingFilePath = Path.Combine(path, fileRelativePath);

                    // if this file does not exist on target path, deployment is necessary
                    if (!existingFiles.Any(f => f.Equals(existingFilePath)))
                    {
                        return true;
                    }

                    // compare md5
                    string fileMD5 = fileManifest[1];

                    using (var md5 = MD5.Create())
                    {
                        using (var fileStream = File.OpenRead(existingFilePath))
                        {
                            var hash = md5.ComputeHash(fileStream);
                            string md5String = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                            if (fileMD5 != md5String)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private Stream GetResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resource = assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(resourceName));
            return resource != null ? assembly.GetManifestResourceStream(resource) : null;
        }
    }
}
