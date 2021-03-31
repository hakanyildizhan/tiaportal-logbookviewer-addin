using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sirius.LogbookViewer.Product
{
    public class FileParser
    {
        private const int DefaultBufferSize = 4096;

        /// <summary>
        /// Indicates that
        /// 1. The file is to be used for asynchronous reading.
        /// 2. The file is to be accessed sequentially from beginning to end.
        /// </summary>
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        /// <summary>
        /// Gets all the data contained in a file.
        /// </summary>
        public static async Task<List<string>> GetDataAsync(string filePath)
        {
            CheckFile(filePath);
            var data = new List<string>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    data.Add(line);
                }
            }

            return data;
        }

        /// <summary>
        /// Throws an <see cref="IOException"/> in case the file does not exist or is being used by another process.
        /// </summary>
        /// <param name="filePath"></param>
        private static void CheckFile(string filePath)
        {
            using (FileStream stream = new FileInfo(filePath).Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
        }
    }
}
