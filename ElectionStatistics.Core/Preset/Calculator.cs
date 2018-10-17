using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Preset
{
    public class Calculator
    {
        private ModelContext modelContext;

        private List<int> lineDescriptionIds;
        private Model.Preset preset;
        // DataTable is for expression evaluation
        private DataTable dataTable = new DataTable();

        public Calculator(ModelContext modelContext, Parser parser, Model.Preset preset)
        {
            this.modelContext = modelContext;
            this.preset = preset;

            lineDescriptionIds = parser.Execute(preset.Expression);
        }

        public List<LineCalculatedValue> Execute()
        {
            var protocols = modelContext.Set<Protocol>().AsNoTracking()
                .Where(protocol => protocol.ProtocolSetId == this.preset.ProtocolSetId)
                    .GroupJoin(modelContext.Set<LineNumber>()
                        .Where(lineNumber =>
                            lineDescriptionIds.Contains(lineNumber.LineDescriptionId)),
                        protocol => protocol.Id,
                        lineNumber => lineNumber.ProtocolId,
                        (protocol, lineNumbers) => new Protocol
                        {
                            Id = protocol.Id, LineNumbers = lineNumbers.ToList()
                        })
                .Where(protocol => protocol.LineNumbers.Count > 0).ToList();
            return protocols.Select(protocol =>
                new LineCalculatedValue()
                {
                    Value = ExecuteSingle(protocol),
                    ProtocolId = protocol.Id,
                    PresetId = preset.Id
                }).ToList();
        }

        private double ExecuteSingle(Protocol protocol)
        {
            var exprBuilder = new StringBuilder();
            var lineNumbers = GetMatchedLineNumbers(protocol);

            var lastIndex = 0;
            for (int i = 0; i < lineDescriptionIds.Count; ++i)
            {
                var index = preset.Expression.IndexOf(lineDescriptionIds[i].ToString());
                exprBuilder.Append(preset.Expression.Substring(lastIndex, index - lastIndex));
                exprBuilder.Append(lineNumbers[i].Value);
                lastIndex = index + lineDescriptionIds[i].ToString().Length;
            }
            exprBuilder.Append(preset.Expression.Substring(lastIndex,
                preset.Expression.Length - lastIndex));

            return Convert.ToDouble(dataTable.Compute(exprBuilder.ToString(), null));
        }

        private List<LineNumber> GetMatchedLineNumbers(Protocol protocol)
        {
            return lineDescriptionIds.Select(lineDescrId =>
                protocol.LineNumbers.Where(line => line.LineDescriptionId == lineDescrId)
                    .SingleOrDefault()).ToList();
        }
    }
}
