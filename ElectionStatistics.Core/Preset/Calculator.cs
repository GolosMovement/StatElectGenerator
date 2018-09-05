using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Preset
{
    public class Calculator
    {
        private ModelContext modelContext;

        private List<int> lineDescriptionIds;
        private string expression;

        public Calculator(ModelContext modelContext, Parser parser, string expression)
        {
            this.modelContext = modelContext;
            this.expression = expression;

            lineDescriptionIds = parser.Execute(expression);
        }

        public double Execute(int protocolId)
        {
            var exprBuilder = new StringBuilder();

            var lineNumbers = GetMatchedLineNumbers(protocolId);

            var lastIndex = 0;
            for (int i = 0; i < lineDescriptionIds.Count; ++i)
            {
                var index = expression.IndexOf(lineDescriptionIds[i].ToString());
                exprBuilder.Append(expression.Substring(lastIndex, index - lastIndex));
                exprBuilder.Append(lineNumbers[i].Value);
                lastIndex = index + lineDescriptionIds[i].ToString().Length;
            }
            exprBuilder.Append(expression.Substring(lastIndex, expression.Length - lastIndex));

            return Dangl.Calculator.Calculator.Calculate(exprBuilder.ToString()).Result;
        }

        private List<LineNumber> GetMatchedLineNumbers(int protocolId)
        {
            return lineDescriptionIds.Select(lineDescrId =>
                modelContext.Set<LineNumber>()
                    .Where(line => line.ProtocolId == protocolId &&
                        line.LineDescriptionId == lineDescrId).SingleOrDefault()).ToList();
        }
    }
}
