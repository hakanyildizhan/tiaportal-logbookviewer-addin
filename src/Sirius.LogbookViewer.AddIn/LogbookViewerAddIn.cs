using System;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;

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
            try
            {
                using (_tiaPortal.ExclusiveAccess("Logbook Viewer is being loaded, please wait..."))
                {
                    string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Siemens AG", "Logbook Viewer AddIn");

                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true);
                    }
                     
                    Directory.CreateDirectory(folder);
                    string packagePath = Path.Combine(folder, "Package.zip");

                    var assembly = Assembly.GetExecutingAssembly();
                    string resourceName = assembly.GetManifestResourceNames().Single(r => r.EndsWith("Package.zip"));
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
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
                    Siemens.Engineering.AddIn.Utilities.Process.Start(Path.Combine(folder, "Sirius.LogbookViewer.App.exe"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.Message + "\r\n" + ex.InnerException);
            }
        }
    }
}
