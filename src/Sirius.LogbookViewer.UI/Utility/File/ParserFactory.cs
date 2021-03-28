using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sirius.LogbookViewer.UI
{
    public static class ParserFactory
    {
        private const int DefaultBufferSize = 4096;
        private const string SAFETY_RELAY = "Safety Relay";
        private const string SOFTSTARTER = "Soft Starter";

        /// <summary>
        /// Indicates that
        /// 1. The file is to be used for asynchronous reading.
        /// 2. The file is to be accessed sequentially from beginning to end.
        /// </summary>
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public static async Task<IParser> GetParser(string filePath)
        {
            CheckFile(filePath);
            ParserType parserType = ParserType.None;
            var columns = new List<string>();
            var rows = new List<List<string>>();
            bool atColumnRow = false;
            int numColumns = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (parserType == ParserType.None)
                    {
                        if (line.Contains(SAFETY_RELAY))
                        {
                            parserType = ParserType.Safety;
                        }
                        else if (line.Contains(SOFTSTARTER))
                        {
                            parserType = ParserType.Softstarter;
                        }
                    }

                    if (columns.Count == 0)
                    {
                        if (!atColumnRow)
                        {
                            atColumnRow = line.Contains(',') && line.Split(',').Where(c => !string.IsNullOrEmpty(c)).Count() > 1;

                            if (atColumnRow)
                            {
                                var possibleColumns = line.Split(',').Where(c => !string.IsNullOrEmpty(c));
                                numColumns = possibleColumns.Count();
                                columns = possibleColumns.ToList();
                            }

                            continue;
                        }
                    }
                    else if (rows.Count == 0)
                    {
                        if (!line.Contains(','))
                        {
                            atColumnRow = false;
                            numColumns = 0;
                            columns.Clear();
                        }
                        else
                        {
                            var rowData = line.Split(',').Where(c => !string.IsNullOrEmpty(c));
                            int numData = rowData.Count();

                            if (numData == numColumns)
                            {
                                rows.Add(rowData.ToList());
                            }
                            else
                            {
                                atColumnRow = false;
                                numColumns = 0;
                                columns.Clear();
                            }
                        }
                    }
                    else
                    {
                        var rowData = line.Split(',').Where(c => !string.IsNullOrEmpty(c));
                        rows.Add(rowData.ToList());
                    }
                }
            }

            if (columns.Count == 0)
            {
                throw new Exception("File is corrupt.");
            }

            var fileMeta = new FileMeta()
            {
                Columns = columns,
                Rows = rows
            };

            switch (parserType)
            {
                case ParserType.Safety:
                    return new SafetyParser(fileMeta);
                case ParserType.Softstarter:
                    return new SoftstarterParser(fileMeta);
                case ParserType.None:
                default:
                    throw new Exception("Unknown Logbook type.");
            }
        }

        private static void CheckFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist.");
            }
            else if (new FileInfo(filePath).Extension != ".csv")
            {
                throw new Exception("File is not in the correct format.");
            }

            try
            {
                using (FileStream stream = new FileInfo(filePath).Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                throw new Exception("File is being used by another process.");
            }
        }
    }
}
