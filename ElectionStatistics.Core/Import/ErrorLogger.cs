using System;
using System.IO;

namespace ElectionStatistics.Core.Import
{
    public class ErrorLogger : IErrorLogger
    {
        private string fileName;

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        public ErrorLogger()
        {
            // TODO: use special log storage for import logs
            this.fileName = Path.GetTempFileName();
        }

        public void Error(int line, int column, string humanColumn,
            string message)
        {
            File.AppendAllText(fileName,
                string.Format("{0}:{1} ({2}): {3}",
                    line, column, humanColumn, message) + Environment.NewLine);
        }
    }
}
