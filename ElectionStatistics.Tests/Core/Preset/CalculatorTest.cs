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
            var protocol = new Protocol();
            modelContext.Add(protocol);

            var lineDescriptions = new List<LineDescription>()
            {
                new LineDescription(), new LineDescription(), new LineDescription()
            };
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
            var preset = new Model.Preset() { Expression = expression };
            modelContext.Add(preset);
            modelContext.SaveChanges();

            var parser = new Parser();

            var service = new Calculator(modelContext, parser, preset.Expression);
            var result = service.Execute(protocol.Id);
            Assert.Equal(1.55, result);

            // Check internal DataTable reuse
            result = service.Execute(protocol.Id);
            Assert.Equal(1.55, result);
        }
    }
}
