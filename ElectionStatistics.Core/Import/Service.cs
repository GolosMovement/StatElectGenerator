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
        private string xlsxFile;
        private ProtocolSet protocolSet;
        private Mapping mapping;
        private List<MappingLine> mappingLines;
        private bool initialized = false;
        private IProgressNotifier notifier;
        private readonly DummyNotifier dummyNotifier = new DummyNotifier();
        // FIXME: see https://github.com/GolosMovement/StatElectGenerator/issues/49
        private const int maxXlsxRowsError = 1048576;
        private const int maxEmptyLinesSeq = 100;

        private class DummyNotifier : IProgressNotifier
        {
            public void Start(int totalLines)
            {
            }

            public void Progress(int currentLine, int errorCount)
            {
            }

            public void Finish(int currentLine, bool success, int errorCount)
            {
            }
        }

        public Service(ISerializer serializer, IErrorLogger errorLogger)
        {
            this.serializer = serializer;
            this.errorLogger = errorLogger;

            // TODO: move this line to Startup.cs, see:
            // https://github.com/ExcelDataReader/ExcelDataReader#important-note-on-net-core
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);
        }

        public Service Configure(string xlsxFile, ProtocolSet protocolSet,
            Mapping mapping, List<MappingLine> mappingLines)
        {
            this.xlsxFile = xlsxFile;
            this.protocolSet = protocolSet;
            this.mapping = mapping;
            this.mappingLines = mappingLines;

            CheckFileExists(xlsxFile);
            CheckProtocolSet(protocolSet);
            CheckMappings(mapping, mappingLines);

            return this;
        }

        public Service Initialize()
        {
            protocolSet.ImportFileErrorLog = errorLogger.GetFileName();
            serializer.CreateProtocolSet(protocolSet);
            initialized = true;

            return this;
        }

        public void Execute()
        {
            Execute(null, null);
        }

        public void Execute(IProgressNotifier notifier = null)
        {
            Execute(null, notifier);
        }

        public void Execute(ISerializer serializer = null,
            IProgressNotifier notifier = null)
        {
            ReinitializeSerialiser(serializer);
            InitializeNotifier(notifier);

            // TODO: use fields instead of arguments
            ImportXlsx(xlsxFile, mapping, protocolSet,
                WrapMappings(mappingLines));
        }

        private void ReinitializeSerialiser(ISerializer serializer)
        {
            if (serializer != null)
            {
                this.serializer = serializer;
            }
        }

        private void InitializeNotifier(IProgressNotifier notifier)
        {
            if (notifier != null)
            {
                this.notifier = notifier;
            }
            else
            {
                this.notifier = dummyNotifier;
            }
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
            bool success = true;
            int line = 0;

            try
            {
                if (!initialized)
                {
                    Initialize();
                }

                serializer.BeforeImport();

                using (var stream = File.Open(xlsxFile, FileMode.Open,
                    FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int totalLines = reader.RowCount;
                        notifier.Start(totalLines);

                        Dictionary<string, Protocol> createdProtocols =
                            new Dictionary<string, Protocol>();

                        // TODO: check reader.FieldCount >=
                        // MaxBy(MappingLine.ColumnNumber)
                        CreateLineDescriptions(protocolSet, allMappings);
                        var hierarchy = WrapHierarchy(allMappings);
                        var mappings = MappingsWithoutHierarchy(allMappings);

                        int emptyLinesSeq = 0;
                        while (reader.Read())
                        {
                            if (totalLines == maxXlsxRowsError &&
                                emptyLinesSeq == maxEmptyLinesSeq)
                            {
                                // Stop if we encountered the issue #49
                                line = totalLines;
                                Error(line, 0, "Stop due to the issue #49");
                                break;
                            }

                            // Skip header lines
                            if (++line < mapping.DataLineNumber)
                            {
                                continue;
                            }

                            var protocol = UpdateHierarchy(line,
                                protocolSet, createdProtocols, hierarchy,
                                reader);

                            if (protocol == null)
                            {
                                emptyLinesSeq++;
                                continue;
                            }

                            if (emptyLinesSeq > 0)
                            {
                                emptyLinesSeq = 0;
                            }

                            CreateLines(line, protocol, mappings, reader);

                            // TODO: progress notifier is not useful because
                            // all the work is on the bulk insertions
                            // right now, see DbSerializer#AfterImport
                            notifier.Progress(line,
                                errorLogger.GetErrorsCount());
                        }

                        serializer.AfterImport();
                    }
                }
            }
            catch (Exception e)
            {
                success = false;
                Error(line, 0, e.ToString());
                throw;
            }
            finally
            {
                notifier.Finish(line, success, errorLogger.GetErrorsCount());
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
                .OrderByDescending(x => x.Key);

            List<HirarchyEnvelope> list = new List<HirarchyEnvelope>();

            foreach (var item in groups)
            {
                var hierarchyItem = new HirarchyEnvelope();

                foreach (var subitem in item)
                {
                    if (subitem.MappingLine.IsNumber)
                    {
                        hierarchyItem.Number = subitem;
                    }
                    else
                    {
                        switch (subitem.MappingLine.HierarchyLanguage)
                        {
                            case LanguageEnum.Russian:
                                hierarchyItem.Russian = subitem;
                                break;

                            case LanguageEnum.English:
                                hierarchyItem.English = subitem;
                                break;

                            default:
                                hierarchyItem.Native = subitem;
                                break;
                        }
                    }
                }

                // TODO: skip invalid hierarchy items
                list.Add(hierarchyItem);
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
                var mappingRus = current.Russian.MappingLine;
                var columnRus = ColumnLine.Dehumanize(
                    mappingRus.ColumnNumber);
                var isLastItem = i == hierarchy.Count - 1;

                var protocolRusTitle = reader.GetValue(columnRus)?.ToString();

                if (!rootCreated && String.IsNullOrEmpty(protocolRusTitle))
                {
                    Error(line, columnRus, "Line has been skipped. " +
                        "It has no root hierarchy");
                    return null;
                }
                else
                if (String.IsNullOrEmpty(protocolRusTitle))
                {
                    // If protocolRusTitle is empty that means a skip inside
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
                if (current.Number != null)
                {
                    var numberColumn =
                        ColumnLine.Dehumanize(
                            current.Number.MappingLine.ColumnNumber);

                    var commType = reader.GetFieldType(numberColumn);
                    if (commType != typeof(System.Double))
                    {
                        Error(line, numberColumn,
                            "Protocol commission number should be a number");
                    }
                    else
                    {
                        protocolCommission = reader.GetDouble(numberColumn);
                    }
                }

                var key = mappingRus.HierarchyLevel.ToString() +
                    protocolRusTitle;

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

                protocol.TitleRus = protocolRusTitle;
                protocol.TitleEng = ReadHierarchyTitle(current.English,
                    reader);
                protocol.TitleNative = ReadHierarchyTitle(current.Native,
                    reader);

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

        private string ReadHierarchyTitle(MappingEnvelope mappingEnv,
            IExcelDataReader reader)
        {
            if (mappingEnv == null)
            {
                return null;
            }

            var title = reader.GetValue(
                ColumnLine.Dehumanize(
                    mappingEnv.MappingLine.ColumnNumber))?.ToString();

            if (String.IsNullOrEmpty(title))
            {
                return null;
            }

            return title;
        }

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
                    else
                    {
                        lineNumber.Value = null;
                    }

                    lineNumber.ProtocolId = protocol.Id;
                    lineNumber.LineDescriptionId = item.LineDescription.Id;

                    serializer.CreateLineNumber(lineNumber);
                }
                else
                {
                    var columnType = reader.GetFieldType(columnNumber);

                    if (columnType == null)
                    {
                        continue;
                    }

                    var lineString = new LineString();
                    lineString.Value = reader.GetValue(columnNumber).ToString();
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
