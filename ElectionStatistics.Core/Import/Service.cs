using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ExcelDataReader;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    // TODO: check equal ColumnNumbers of two or more MappingLine
    public class Service
    {
        private ISerializer serializer;
        private IErrorLogger errorLogger;

        // TODO: create special default builder class (pattern)
        public Service(ISerializer serializer, IErrorLogger errorLogger)
        {
            this.serializer = serializer;
            this.errorLogger = errorLogger;
        }

        // TODO: add public check method - evaluate mappings (types) and
        // real data
        public void Execute(string xlsxFile, ProtocolSet protocolSet,
            Mapping mapping, List<MappingLine> mappingLines)
        {
            CheckFileExists(xlsxFile);
            CheckProtocolSet(protocolSet);
            CheckMappings(mapping, mappingLines);

            ImportXlsx(xlsxFile, mapping, protocolSet,
                WrapMappings(mappingLines));
        }

        private void CheckFileExists(string xlsxFile)
        {
            if (!File.Exists(xlsxFile))
            {
                throw new ImportException(string.Format(
                    "File '{0}' doesn't exist", xlsxFile));
            }
        }

        private void CheckProtocolSet(ProtocolSet protocolSet)
        {
            // TODO: check TitleRus and DescriptionRus peresence
            if (protocolSet == null)
            {
                throw new ImportException("ProtocolSet is not provided");
            }
        }

        private void CheckMappings(Mapping mapping,
            List<MappingLine> mappingLines)
        {
            if (mapping == null)
            {
                throw new ImportException("Mapping is not provided");
            }

            if (mappingLines == null || mappingLines.Count == 0)
            {
                throw new ImportException("Mapping lines are not provided");
            }
            // TODO: check sanity of mappings
        }

        private List<MappingEnvelope> WrapMappings(
            List<MappingLine> mappingLines)
        {
            return mappingLines.Select(
                x => new MappingEnvelope() { MappingLine = x } ).ToList();
        }

        private void ImportXlsx(string xlsxFile,
            Mapping mapping,
            ProtocolSet protocolSet,
            List<MappingEnvelope> allMappings)
        {
            // TODO: move this line to Startup.cs, see:
            // https://github.com/ExcelDataReader/ExcelDataReader#important-note-on-net-core
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(xlsxFile, FileMode.Open,
                FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    Dictionary<string, Protocol> createdProtocols =
                        new Dictionary<string, Protocol>();

                    // TODO: check reader.FieldCount >=
                    // MaxBy(MappingLine.ColumnNumber)
                    serializer.CreateProtocolSet(protocolSet);
                    CreateLineDescriptions(protocolSet, allMappings);
                    var hierarchy = WrapHierarchy(allMappings);
                    var mappings = MappingsWithoutHierarchy(allMappings);

                    do
                    {
                        int line = 0;
                        while (reader.Read())
                        {
                            // Skip header lines
                            if (++line < mapping.DataLineNumber)
                            {
                                continue;
                            }

                            var protocol = UpdateHierarchy(line, protocolSet,
                                createdProtocols, hierarchy, reader);

                            if (protocol == null)
                            {
                                continue;
                            }

                            CreateLines(line, protocol, mappings, reader);
                        }
                    } while (reader.NextResult());

                    serializer.AfterImport();
                }
            }
        }

        private void CreateLineDescriptions(ProtocolSet protocolSet,
            List<MappingEnvelope> allMappings)
        {
            foreach (var item in allMappings)
            {
                item.LineDescription = new LineDescription()
                {
                    TitleRus = item.MappingLine.TitleRus,
                    TitleEng = item.MappingLine.TitleEng,
                    TitleNative = item.MappingLine.TitleNative,

                    DescriptionRus = item.MappingLine.DescriptionRus,
                    DescriptionEng = item.MappingLine.DescriptionEng,
                    DescriptionNative = item.MappingLine.DescriptionNative,

                    IsVoteResult = item.MappingLine.IsVoteResult,
                    IsCalcResult = item.MappingLine.IsCalcResult,

                    ProtocolSetId = protocolSet.Id
                };

                serializer.CreateLineDescription(item.LineDescription);
            }
        }

        private List<HirarchyEnvelope> WrapHierarchy(
            List<MappingEnvelope> allMappings)
        {
            var groups = allMappings
                .Where(x => x.MappingLine.IsHierarchy)
                .GroupBy(x => x.MappingLine.HierarchyLevel)
                .OrderBy(x => x.Key);

            List<HirarchyEnvelope> list = new List<HirarchyEnvelope>();

            foreach (var item in groups)
            {
                var str = item
                    .Where(x => !x.MappingLine.IsNumber)
                    .SingleOrDefault();
                var number = item
                    .Where(x => x.MappingLine.IsNumber)
                    .SingleOrDefault();

                if (str == null)
                {
                    continue;
                }

                var envelope = new HirarchyEnvelope() { StringValue = str };

                if (number != null)
                {
                    envelope.NumberValue = number;
                }

                list.Add(envelope);
            }

            return list;
        }

        private List<MappingEnvelope> MappingsWithoutHierarchy(
            List<MappingEnvelope> allMappings)
        {
            return allMappings
                .Where(x => !x.MappingLine.IsHierarchy)
                .OrderBy(x => x.MappingLine.ColumnNumber)
                .ToList();
        }

        private Protocol UpdateHierarchy(
            int line,
            ProtocolSet protocolSet,
            Dictionary<string, Protocol> createdProtocols,
            List<HirarchyEnvelope> hierarchy,
            IExcelDataReader reader)
        {
            Protocol parent = null;
            var rootCreated = false;

            for (int i = 0; i < hierarchy.Count; i++)
            {
                var current = hierarchy[i];
                var mapping = current.StringValue.MappingLine;
                var stringColumn = ColumnLine.Dehumanize(mapping.ColumnNumber);
                var isLastItem = i == hierarchy.Count - 1;

                var titleType = reader.GetFieldType(stringColumn);
                if (titleType != null && titleType != typeof(System.String))
                {
                    if (!rootCreated)
                    {
                        Error(line, stringColumn,
                            "Line has been skipped. " +
                            "Protocol title should be a string. " +
                            "No root hierarchy");
                        return null;
                    }
                    else
                    {
                        Error(line, stringColumn,
                            "Protocol title should be a string");
                        continue;
                    }
                }

                var protocolTitle = reader.GetString(stringColumn);

                if (!rootCreated && String.IsNullOrEmpty(protocolTitle))
                {
                    Error(line, stringColumn, "Line has been skipped. " +
                        "It has no root hierarchy");
                    return null;
                }
                else if (String.IsNullOrEmpty(protocolTitle))
                {
                    // If protocolTitle is empty that means a skip inside
                    // the hierarchy
                    if (isLastItem)
                    {
                        // Last item can't be skipped, return parent Protocol
                        return parent;
                    }
                    else
                    {
                        // Hierarchy skips can be only in between first and
                        // last items
                        continue;
                    }
                }

                double protocolCommission = 0.0;
                if (current.NumberValue != null)
                {
                    var numberColumn =
                        ColumnLine.Dehumanize(
                            current.NumberValue.MappingLine.ColumnNumber);

                    var commType = reader.GetFieldType(numberColumn);
                    if (commType != typeof(System.Double))
                    {
                        Error(line, numberColumn,
                            "Protocol commission number should be a number");
                    } else {
                        protocolCommission = reader.GetDouble(numberColumn);
                    }
                }

                var key = mapping.HierarchyLevel.ToString() + protocolTitle;

                // Last item in the hierarchy is always created
                if (!isLastItem)
                {
                    if (createdProtocols.ContainsKey(key))
                    {
                        parent = createdProtocols[key];

                        if (!rootCreated)
                        {
                            rootCreated = true;
                        }

                        continue;
                    }
                }

                var protocol = new Protocol();

                protocol.TitleRus = protocolTitle;
                protocol.ProtocolSetId = protocolSet.Id;

                if (protocolCommission > 0.0)
                {
                    protocol.CommissionNumber = Convert.ToInt32(
                        protocolCommission);
                }

                if (parent != null)
                {
                    protocol.ParentId = parent.Id;
                }

                serializer.CreateProtocol(protocol);
                createdProtocols[key] = protocol;

                if (!rootCreated)
                {
                    rootCreated = true;
                }

                parent = protocol;
            }

            return parent;
        }

        // TODO: inherit LineNumber and LineString from a base class for DRY
        private void CreateLines(int line, Protocol protocol,
            List<MappingEnvelope> mappings, IExcelDataReader reader)
        {
            foreach (var item in mappings)
            {
                var mapping = item.MappingLine;
                var columnNumber = ColumnLine.Dehumanize(mapping.ColumnNumber);

                if (mapping.IsNumber)
                {
                    var columnType = reader.GetFieldType(columnNumber);

                    if (columnType != null &&
                        columnType != typeof(System.Double))
                    {
                        Error(line, columnNumber,
                            String.Format(
                                "Cell value should be a number. " +
                                "Current type is \"{0}\"",
                                columnType));
                        continue;
                    }

                    var lineNumber = new LineNumber();
                    if (columnType != null)
                    {
                        lineNumber.Value = Convert.ToInt32(
                            reader.GetDouble(columnNumber));
                    }

                    lineNumber.ProtocolId = protocol.Id;
                    lineNumber.LineDescriptionId = item.LineDescription.Id;
                    serializer.CreateLineNumber(lineNumber);
                }
                else
                {
                    var columnType = reader.GetFieldType(columnNumber);

                    if (columnType != null &&
                        columnType != typeof(System.String))
                    {
                        Error(line, columnNumber,
                            String.Format(
                                "Cell value should be a string. " +
                                "Current type is \"{0}\"",
                                columnType));
                        continue;
                    }

                    var lineString = new LineString();
                    if (columnType != null)
                    {
                        lineString.Value = reader.GetString(columnNumber);
                    }

                    lineString.ProtocolId = protocol.Id;
                    lineString.LineDescriptionId = item.LineDescription.Id;
                    serializer.CreateLineString(lineString);
                }
            }
        }

        private void Error(int line, int column, string message)
        {
            errorLogger.Error(line, ColumnLine.Humanize(column),
                ColumnLine.ToLetters(column), message);
        }
    }
}
