using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Core.Preset;
using ElectionStatistics.Model;

namespace ElectionStatistics.Tests.Core.Preset
{
    public class CalculatorTest
    {
        private ModelContext modelContext;

        public CalculatorTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ModelContext>();
            optionsBuilder.UseInMemoryDatabase(this.GetType().ToString());
            modelContext = new ModelContext(optionsBuilder.Options);
        }

        [Fact]
        public void Execute_WithParenthesis_EvaluatesExpression()
        {
            var protocolSet = new ProtocolSet();
            modelContext.Add(protocolSet);
            var protocol = new Protocol() { ProtocolSetId = protocolSet.Id };
            modelContext.Add(protocol);

            var lineDescriptions = Enumerable.Range(1, 3).Select((n) =>
                new LineDescription() { ProtocolSetId = protocolSet.Id }).ToList();
            lineDescriptions.ForEach(lineDes => modelContext.Add(lineDes));

            var lineNumbers = new List<LineNumber>()
            {
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[1].Id, Value = 12, ProtocolId = protocol.Id
                },
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[0].Id, Value = 50, ProtocolId = protocol.Id
                },
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[2].Id, Value = 40, ProtocolId = protocol.Id
                }
            };
            lineNumbers.ForEach(lineNum => modelContext.Add(lineNum));

            var expression = $"(({lineDescriptions[0].Id} + {lineDescriptions[1].Id}) / " +
                $"{lineDescriptions[2].Id})";
            var preset = new Model.Preset()
            {
                Expression = expression, ProtocolSetId = protocolSet.Id
            };
            modelContext.Add(preset);
            modelContext.SaveChanges();

            var parser = new Parser();
            var service = new Calculator(modelContext, parser, preset);
            var result = service.Execute();
            var expectedVal = new LineCalculatedValue()
            {
                Value = 1.55, PresetId = preset.Id, ProtocolId = protocolSet.Id
            };
            Assert.Collection(result, item =>
            {
                Assert.Equal(expectedVal.Value, item.Value);
                Assert.Equal(expectedVal.PresetId, item.PresetId);
                Assert.Equal(expectedVal.ProtocolId, item.ProtocolId);
            });
        }
    }
}
