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

            var protocol2 = new Protocol() { ProtocolSetId = protocolSet.Id };
            modelContext.Add(protocol2);

            var lineDescriptions = Enumerable.Range(1, 3).Select((n) =>
                new LineDescription() { ProtocolSetId = protocolSet.Id }).ToList();
            modelContext.AddRange(lineDescriptions);

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
                },

                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[1].Id, Value = 10, ProtocolId = protocol2.Id
                },
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[0].Id, Value = 13, ProtocolId = protocol2.Id
                },
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[2].Id, Value = null, ProtocolId = protocol2.Id
                }
            };
            modelContext.AddRange(lineNumbers);

            var expression = $"(([{lineDescriptions[0].Id}] + [{lineDescriptions[1].Id}]) / " +
                $"[{lineDescriptions[2].Id}])";
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
                Value = 1.55, PresetId = preset.Id, ProtocolId = protocol.Id
            };

            var expectedValNull = new LineCalculatedValue()
            {
                Value = null, PresetId = preset.Id, ProtocolId = protocol2.Id
            };

            Assert.Collection(result, item =>
            {
                Assert.Equal(expectedVal.Value, item.Value);
                Assert.Equal(expectedVal.PresetId, item.PresetId);
                Assert.Equal(expectedVal.ProtocolId, item.ProtocolId);
            },
            item =>
            {
                Assert.Equal(expectedValNull.Value, item.Value);
                Assert.Equal(expectedValNull.PresetId, item.PresetId);
                Assert.Equal(expectedValNull.ProtocolId, item.ProtocolId);
            });
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        public void Execute_DivisionByZero_EvaluatesExpressionToZero(int valA, int valB)
        {
            var protocolSet = new ProtocolSet();
            modelContext.Add(protocolSet);
            var protocol = new Protocol() { ProtocolSetId = protocolSet.Id };
            modelContext.Add(protocol);

            var lineDescriptions = Enumerable.Range(1, 2).Select((n) =>
                new LineDescription() { ProtocolSetId = protocolSet.Id }).ToList();
            modelContext.AddRange(lineDescriptions);

            var lineNumbers = new List<LineNumber>()
            {
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[0].Id,
                    Value = valA,
                    ProtocolId = protocol.Id
                },
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[1].Id,
                    Value = valB,
                    ProtocolId = protocol.Id
                }
            };
            modelContext.AddRange(lineNumbers);

            var expression = $"[{lineDescriptions[0].Id}] / [{lineDescriptions[1].Id}]";
            var preset = new Model.Preset()
            {
                Expression = expression,
                ProtocolSetId = protocolSet.Id
            };
            modelContext.Add(preset);
            modelContext.SaveChanges();

            var parser = new Parser();
            var service = new Calculator(modelContext, parser, preset);
            var result = service.Execute();

            Assert.Collection(result, item => Assert.Equal(null, item.Value));
        }

        [Fact]
        public void Execute_RepeatableIds_EvaluatesExpression()
        {
            var protocolSet = new ProtocolSet();
            modelContext.Add(protocolSet);
            var protocol = new Protocol() { ProtocolSetId = protocolSet.Id };
            modelContext.Add(protocol);

            var lineDescription = new LineDescription() { ProtocolSetId = protocolSet.Id };
            modelContext.Add(lineDescription);

            var lineNumber = new LineNumber()
            {
                LineDescriptionId = lineDescription.Id, Value = 12, ProtocolId = protocol.Id
            };
            modelContext.Add(lineNumber);

            var expression = $"[{lineDescription.Id}] + [{lineDescription.Id}]";
            var preset = new Model.Preset()
            {
                Expression = expression,
                ProtocolSetId = protocolSet.Id
            };
            modelContext.Add(preset);
            modelContext.SaveChanges();

            var parser = new Parser();
            var service = new Calculator(modelContext, parser, preset);
            var result = service.Execute();
            var expectedVal = new LineCalculatedValue()
            {
                Value = 24,
                PresetId = preset.Id,
                ProtocolId = protocol.Id
            };
            Assert.Collection(result, item =>
            {
                Assert.Equal(expectedVal.Value, item.Value);
                Assert.Equal(expectedVal.PresetId, item.PresetId);
                Assert.Equal(expectedVal.ProtocolId, item.ProtocolId);
            });
        }

        [Fact]
        public void Execute_WithNonReplaceable_EvaluatesExpression()
        {
            var protocolSet = new ProtocolSet();
            modelContext.Add(protocolSet);
            var protocol = new Protocol() { ProtocolSetId = protocolSet.Id };
            modelContext.Add(protocol);

            var lineDescriptions = Enumerable.Range(1, 2).Select((n) =>
                new LineDescription() { ProtocolSetId = protocolSet.Id }).ToList();
            modelContext.AddRange(lineDescriptions);

            var lineNumbers = new List<LineNumber>()
            {
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[0].Id, Value = 12, ProtocolId = protocol.Id
                },
                new LineNumber()
                {
                    LineDescriptionId = lineDescriptions[1].Id, Value = 50, ProtocolId = protocol.Id
                }
            };
            modelContext.AddRange(lineNumbers);

            var expression = $"[{lineDescriptions[0].Id}] / [{lineDescriptions[1].Id}] * 100";
            var preset = new Model.Preset()
            {
                Expression = expression,
                ProtocolSetId = protocolSet.Id
            };
            modelContext.Add(preset);
            modelContext.SaveChanges();

            var parser = new Parser();
            var service = new Calculator(modelContext, parser, preset);
            var result = service.Execute();
            var expectedVal = new LineCalculatedValue()
            {
                Value = 24,
                PresetId = preset.Id,
                ProtocolId = protocol.Id
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
