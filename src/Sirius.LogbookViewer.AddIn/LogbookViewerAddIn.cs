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
using System.Resources;
using System.Collections.Generic;

namespace Sirius.LogbookViewer.AddIn
{
    public class LogbookViewerAddIn : ContextMenuAddIn
    {
        private readonly TiaPortal _tiaPortal;
        private readonly ResourceManager _resourceManager;
        private static readonly List<string> AVAILABLE_CULTURES = new List<string>() { "en", "de-DE", "fr-FR" };

        public LogbookViewerAddIn(TiaPortal tiaPortal) : base("Logbook Viewer")
        {
            _tiaPortal = tiaPortal;
            _resourceManager = new ResourceManager("Sirius.LogbookViewer.AddIn.Resources.Resource", Assembly.GetAssembly(typeof(LogbookViewerAddIn)));
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Offline logbook viewer...", OnClick);
        }

        private void OnClick(MenuSelectionProvider menuSelectionProvider)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Siemens AG", "Logbook Viewer AddIn");
            CultureInfo currentCulture = null;

            try
            {
                currentCulture = (CultureInfo)_tiaPortal.SettingsFolders[0].Settings.First(s => s.Name.Equals("UserInterfaceLanguage")).Value;

                if (!AVAILABLE_CULTURES.Any(c => currentCulture.Name.StartsWith(c)))
                {
                    currentCulture = new CultureInfo("en-US");
                }

            }
            catch (Exception)
            {
                currentCulture = new CultureInfo("en-US");
            }

            try
            {
                using (_tiaPortal.ExclusiveAccess(_resourceManager.GetString("WaitMessage", currentCulture)))
                {
                    if (IsDeployNecessary(folder))
                    {
                        if (Directory.Exists(folder))
                        {
                            Directory.Delete(folder, true);
                        }
                        
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

                    Siemens.Engineering.AddIn.Utilities.Process.Start(Path.Combine(folder, "Sirius.LogbookViewer.App.exe"), $"--culture {currentCulture.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(_resourceManager.GetString("StartErrorMessage", currentCulture), _resourceManager.GetString("Error", currentCulture), MessageBoxButton.OK, MessageBoxImage.Error);
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
