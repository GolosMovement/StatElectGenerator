using System;
using Microsoft.EntityFrameworkCore;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    //
    // TODO: test on real data import (~100k lines)
    // TODO: in memory DB tests
    //
    // Example of DbSerializer usage in context of ServiceTest class:
    //
    // var connectionString = "Server=tcp:localhost,1433;Initial Catalog=ElectionStatistics;Persist Security Info=False;User ID=SA;Password=_PASSWORD_;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";
    // var optionsBuilder = new DbContextOptionsBuilder<ModelContext>();
    // optionsBuilder.UseSqlServer(connectionString);
    //
    // var dbSerializer = new DbSerializer(
    //     new ModelContext(optionsBuilder.Options));
    //
    // var realService = new Service(dbSerializer, errorLogger);
    //
    // var mapping = new Mapping();
    // mapping.DataLineNumber = 2;
    //
    // var mappingList = new List<MappingLine>()
    // {
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("A"),
    //         IsNumber = false, TitleRus = "link" },
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("I"),
    //         IsNumber = true, TitleRus = "1" },
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("AB"),
    //         IsNumber = true, TitleRus = "20" },
    //
    //     // Hierarchy:
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("B"),
    //         IsNumber = true, IsHierarchy = true, HierarchyLevel = 3,
    //         TitleRus = "uik" },
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("C"),
    //         IsNumber = false, IsHierarchy = true, HierarchyLevel = 1,
    //         TitleRus = "kom1" },
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("D"),
    //         IsNumber = false, IsHierarchy = true, HierarchyLevel = 2,
    //         TitleRus = "kom2" },
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("E"),
    //         IsNumber = false, IsHierarchy = true, HierarchyLevel = 3,
    //         TitleRus = "kom3" },
    //     new MappingLine() { ColumnNumber = ColumnLine.FromLetters("F"),
    //         IsNumber = true, IsHierarchy = true, HierarchyLevel = 1,
    //         TitleRus = "kom1nmb" }
    // };
    //
    // realService.Execute(testFile,
    //     new ProtocolSet() { TitleRus = "Hello", DescriptionRus = "P" },
    //     new Mapping() { DataLineNumber = 2 },
    //     mappingList);
    //
    // System.IO.File.WriteAllText("real_errors.json",
    //     ToJson(errorLogger.Errors));
    //
    public class DbSerializer : ISerializer
    {
        private DbContext context;
        private int commitCount = 0;

        public DbSerializer(DbContext context)
        {
            this.context = context;
            context.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public void CreateProtocolSet(ProtocolSet protocolSet)
        {
            context.Set<ProtocolSet>().Add(protocolSet);
            context.SaveChanges();
        }

        public void CreateLineDescription(LineDescription lineDescription)
        {
            context.Set<LineDescription>().Add(lineDescription);
            context.SaveChanges();
        }

        public void CreateProtocol(Protocol protocol)
        {
            context.Set<Protocol>().Add(protocol);
            BulkSave();
        }

        public void CreateLineNumber(LineNumber lineNumber)
        {
            context.Set<LineNumber>().Add(lineNumber);
            BulkSave();
        }

        public void CreateLineString(LineString lineString)
        {
            context.Set<LineString>().Add(lineString);
            BulkSave();
        }

        public void AfterImport()
        {
            context.SaveChanges();
        }

        private void BulkSave()
        {
            commitCount++;

            if (commitCount % 1000 == 0)
            {
                context.SaveChanges();
            }
        }
    }
}
