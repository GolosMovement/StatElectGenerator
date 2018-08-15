using System;
using System.IO;
using System.Linq;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    public class Service
    {
        public void Execute(string csvFile,
            IQueryable<MappingLine> mappingLines)
        {
            CheckFileExists(csvFile);
            CheckMappingLines(mappingLines);
            ImportCsv(csvFile, mappingLines);
        }

        private void CheckFileExists(string csvFile)
        {
            if (!File.Exists(csvFile))
            {
                throw new ImportException(string.Format(
                    "File '{0}' doesn't exist", csvFile));
            }
        }

        private void CheckMappingLines(IQueryable<MappingLine> mappingLines)
        {
            // TODO
        }

        private void ImportCsv(string csvFile,
            IQueryable<MappingLine> mappingLines)
        {
            
        }
    }
}
