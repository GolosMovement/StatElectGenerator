using System;
using Xunit;
using ElectionStatistics.Core.Import;

namespace ElectionStatistics.Tests
{
    public class ImportTest
    {
        private Service service = new Service();

        [Fact]
        public void CsvFileNotFound()
        {
            Assert.Throws<ImportException>(
                () => service.Execute("fake", null));
        }

        [Fact]
        public void EmptyMappingsOrNull()
        {
            service.Execute("./Resources/import_data.csv", null);
        }
    }
}
