using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace Sirius.LogbookViewer.Publisher
{
    class Program
    {
        private const string ARG_PRODUCT_ASSEMBLY = "--productassembly";
        private const string ARG_TIA_ROOT = "--tiaroot";

        /// <summary>
        /// Packages necessary components and creates the TIA AddIn.
        /// Example command:
        /// --productassembly "..\..\..\Sirius.LogbookViewer.Safety\bin\Debug\Sirius.LogbookViewer.Safety.dll" --tiaroot "D:\WS\T17_SIRIUS\binaries\Debug\x64"
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (args.Length != 4 || !args.Any(a => a.Equals(ARG_TIA_ROOT)) || !args.Any(a => a.Equals(ARG_PRODUCT_ASSEMBLY)))
            {
                Trace.TraceError("Invalid arguments.");
                Environment.Exit(1);
            }

            string productAssemblyPath = string.Empty;
            string tiaRootPath = string.Empty;

            for (int i = 0; i < args.Length; i+=2)
            {
                if (args[i].Equals(ARG_TIA_ROOT))
                {
                    tiaRootPath = Path.Combine(Directory.GetCurrentDirectory(), args[i + 1]);
                }
                else if (args[i].Equals(ARG_PRODUCT_ASSEMBLY))
                {
                    productAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), args[i + 1]);
                }
            }

            if (string.IsNullOrEmpty(productAssemblyPath) || string.IsNullOrEmpty(tiaRootPath) ||
                !File.Exists(productAssemblyPath) || !Directory.Exists(tiaRootPath))
            {
                Trace.TraceError("Invalid path.");
                Environment.Exit(1);
            }

            string publisherPath = Path.Combine(tiaRootPath, @"PublicAPI\V17.AddIn\Siemens.Engineering.AddIn.Publisher.exe");

            if (!File.Exists(publisherPath))
            {
                Trace.TraceError("AddIn Publisher is not found within given TIA installation.");
                Environment.Exit(1);
            }

            string activeConfig = string.Empty;
#if DEBUG
            activeConfig = "Debug";
#else
            activeConfig = "Release";
#endif

            string buildOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Publish");
            if (Directory.Exists(buildOutputPath))
            {
                Directory.Delete(buildOutputPath, true);
            }
            Directory.CreateDirectory(buildOutputPath);

            var globalProperty = new Dictionary<string, string>();
            globalProperty.Add("Configuration", activeConfig);
            globalProperty.Add("Platform", "Any CPU");
            globalProperty.Add("OutputPath", buildOutputPath);
            globalProperty.Add("Prefer32Bit", "false");

            var buildParameters = new BuildParameters(new ProjectCollection()) { Loggers = new List<ILogger> { new ConsoleLogger() } };

            // build the app
            Trace.TraceInformation("*****Building Application*****");
            var buildRequest = new BuildRequestData(@"..\..\..\Sirius.LogbookViewer.App\Sirius.LogbookViewer.App.csproj", globalProperty, "15.0", new[] { "Build" }, null);
            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

            if (buildResult.OverallResult == BuildResultCode.Failure)
            {
                Trace.TraceError("Build of AddIn project failed.");
                Environment.Exit(1);
            }

            // get the product specific assembly (and all related output within the same folder)
            string sourceFolder = Directory.GetParent(productAssemblyPath).FullName;
            List<string> productSpecificFiles = Directory.GetFiles(sourceFolder, "*.*", SearchOption.AllDirectories).ToList();

            foreach (string dirPath in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceFolder, buildOutputPath));
            }

            foreach (var f in productSpecificFiles)
            {
                File.Copy(f, f.Replace(sourceFolder, buildOutputPath));
            }

            // create app package & send to AddIn project directory
            string packagePath = "Package.zip";

            if (File.Exists(packagePath))
            {
                File.Delete(packagePath);
            }

            ZipFile.CreateFromDirectory(buildOutputPath, packagePath);
            string packateTargetPath = @"..\..\..\Sirius.LogbookViewer.AddIn\Package\Package.zip";
            
            if (File.Exists(packateTargetPath))
            {
                File.Delete(packateTargetPath);
            }

            File.Move(packagePath, @"..\..\..\Sirius.LogbookViewer.AddIn\Package\Package.zip");

            // build AddIn project with the embedded resource
            Trace.TraceInformation("*****Building AddIn Project*****");
            globalProperty.Remove("Prefer32Bit");
            buildRequest = new BuildRequestData(@"..\..\..\Sirius.LogbookViewer.AddIn\Sirius.LogbookViewer.AddIn.csproj", globalProperty, "15.0", new[] { "Build" }, null);
            buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

            if (buildResult.OverallResult == BuildResultCode.Failure)
            {
                Trace.TraceError("Build of AddIn project failed.");
                Environment.Exit(1);
            }

            // Create the AddIn file
            Trace.TraceInformation("*****Creating AddIn*****");
            string addinFileName = Path.GetFileNameWithoutExtension(productAssemblyPath) + ".addin";
            string addinFilePath = Path.Combine(buildOutputPath, addinFileName);
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(publisherPath);
            p.StartInfo.Arguments = $@"--configuration {new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", $"Configuration.{activeConfig}.xml")).FullName} --outfile {addinFilePath} --console --verbose";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            while (!p.StandardOutput.EndOfStream)
            {
                sb.AppendLine(p.StandardOutput.ReadLine());
            }

            p.WaitForExit();
            Trace.TraceInformation(sb.ToString());

            // send AddIn to TIA installation directory
            Trace.TraceInformation("*****Copying AddIn to Target*****");
            string tiaAddInFolderPath = Path.Combine(tiaRootPath, "AddIns");
            Directory.CreateDirectory(tiaAddInFolderPath);
            File.Copy(addinFilePath, Path.Combine(tiaAddInFolderPath, addinFileName), true);
            Trace.TraceInformation($"Addin file is copied to {tiaAddInFolderPath}");

            stopwatch.Stop();
            Trace.TraceInformation("Completed. Duration: " + TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds + " seconds");
            Environment.Exit(0);
        }
    }
}
