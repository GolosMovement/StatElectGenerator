using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Xunit;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using ElectionStatistics.Model;
using ElectionStatistics.Core.Import;

namespace ElectionStatistics.Tests.Core.Import
{
    // TODO: more test xlsx use cases
    public class ServiceTest
    {
        private class Serializer : ISerializer
        {
            private int id = 1;
            private Dictionary<string, List<object>> data =
                new Dictionary<string, List<object>>();

            public Dictionary<string, List<object>> Data
            {
                get { return data; }
            }

            public void CreateProtocolSet(ProtocolSet protocolSet)
            {
                protocolSet.Id = GetId();
                AddItem(protocolSet);
            }

            public void CreateLineDescription(LineDescription lineDescription)
            {
                lineDescription.Id = GetId();
                AddItem(lineDescription);
            }

            public void CreateProtocol(Protocol protocol)
            {
                protocol.Id = GetId();
                AddItem(protocol);
            }

            public void CreateLineNumber(LineNumber lineNumber)
            {
                lineNumber.Id = GetId();
                AddItem(lineNumber);
            }

            public void CreateLineString(LineString lineString)
            {
                lineString.Id = GetId();
                AddItem(lineString);
            }

            public void AfterImport()
            {
                AddItem("AfterImport");
            }

            private int GetId()
            {
                return id++;
            }

            private void AddItem(object obj)
            {
                var key = obj.GetType().ToString();

                if (!data.ContainsKey(key))
                {
                    data[key] = new List<object>();
                }

                data[key].Add(obj);
            }
        }

        private class ErrorLogger : IErrorLogger
        {
            private List<string> errors = new List<string>();

            public List<string> Errors
            {
                get
                {
                    return errors;
                }
            }

            public void Error(int line, int column, string humanColumn,
                string message)
            {
                errors.Add(String.Format("{0}:{1} ({2}): {3}", line, column,
                    humanColumn, message));
            }
        }

        private static Serializer serializer = new Serializer();
        private static ErrorLogger errorLogger = new ErrorLogger();

        private Service service = new Service(serializer, errorLogger);
        private string testFile =
            Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory +
                "../../../Resources/import_data.xlsx");

        [Fact]
        public void InputFileNotFound()
        {
            Assert.Throws<ImportException>(
                () => service.Execute("fake", null, null, null));
        }

        [Fact]
        public void EmptyMappingsOrNull()
        {
            Assert.Throws<ImportException>(
                () => service.Execute(testFile, new ProtocolSet(), null, null));
            Assert.Throws<ImportException>(
                () => service.Execute(testFile, new ProtocolSet(),
                    new Mapping(), null));
            Assert.Throws<ImportException>(
                () => service.Execute(testFile, null, null, null));
        }

        [Fact]
        public void SomeActualWork()
        {
            var mapping = new Mapping();
            mapping.DataLineNumber = 2;

            var mappingList = new List<MappingLine>()
            {
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("A"),
                    IsNumber = false, TitleRus = "link" },
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("I"),
                    IsNumber = true, TitleRus = "1" },
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("AB"),
                    IsNumber = true, TitleRus = "20" },

                // Hierarchy:
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("B"),
                    IsNumber = true, IsHierarchy = true, HierarchyLevel = 3,
                    TitleRus = "uik" },
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("C"),
                    IsNumber = false, IsHierarchy = true, HierarchyLevel = 1,
                    TitleRus = "kom1" },
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("D"),
                    IsNumber = false, IsHierarchy = true, HierarchyLevel = 2,
                    TitleRus = "kom2" },
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("E"),
                    IsNumber = false, IsHierarchy = true, HierarchyLevel = 3,
                    TitleRus = "kom3" },
                new MappingLine() { ColumnNumber = ColumnLine.FromLetters("F"),
                    IsNumber = true, IsHierarchy = true, HierarchyLevel = 1,
                    TitleRus = "kom1nmb" }
            };

            service.Execute(testFile,
                new ProtocolSet() { TitleRus = "Hello", DescriptionRus = "P" },
                new Mapping() { DataLineNumber = 2 },
                mappingList);

            var actualResultFile = "test-import_data.json";
            System.IO.File.WriteAllText(actualResultFile,
                ToJson(serializer.Data));

            var expectResultFile =
                Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory +
                    "../../../Resources/import_data.json");

            Assert.Equal(MD5Hash(expectResultFile), MD5Hash(actualResultFile));

            var actualErrors = "test-import_errors.json";
            System.IO.File.WriteAllText(actualErrors,
                ToJson(errorLogger.Errors));

            var expectErrors =
                Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory +
                    "../../../Resources/import_errors.json");

            Assert.Equal(MD5Hash(expectErrors), MD5Hash(actualErrors));
        }

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        private string MD5Hash(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream))
                        .Replace("-", "").ToLower();
                }
            }
        }
    }
}
