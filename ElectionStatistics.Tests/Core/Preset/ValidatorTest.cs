using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;

using ElectionStatistics.Core.Preset;
using ElectionStatistics.Model;

namespace ElectionStatistics.Tests.Core.Preset
{
    public class ValidatorTest
    {
        private Validator service;
        private ModelContext modelContext;

        public ValidatorTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ModelContext>();
            optionsBuilder.UseInMemoryDatabase(this.GetType().ToString());
            modelContext = new ModelContext(optionsBuilder.Options);
        }

        [Fact]
        public void Execute_OnNullProtocolSet_ThrowsException()
        {
            Assert.Throws<ValidationException>(() => Service().Execute("237", null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ABC")]
        public void Execute_OnInvalidExpression_ThrowsException(string expression)
        {
            Assert.Throws<ValidationException>(() => Service().Execute(expression, null));
        }

        [Fact]
        public void Execute_OnNotFoundLineDescription_ThrowsException()
        {
            var protocolSet = new ProtocolSet();
            var lineDescription = new LineDescription();

            Assert.Throws<ValidationException>(() =>
                Service().Execute($"{lineDescription.Id}", protocolSet));
        }

        [Fact]
        public void Execute_OnValidInput_ReturnsNothing()
        {
            var protocolSet = new ProtocolSet();
            modelContext.Set<ProtocolSet>().Add(protocolSet);

            var lineDescriptions = new List<LineDescription>()
            {
                new LineDescription() { ProtocolSetId = protocolSet.Id },
                new LineDescription() { ProtocolSetId = protocolSet.Id }
            };
            lineDescriptions.ForEach(lineDes => modelContext.Set<LineDescription>().Add(lineDes));

            modelContext.SaveChanges();

            Service().Execute(string.Join('+', lineDescriptions.Select(lineDes => lineDes.Id)),
                protocolSet);
        }

        private Validator Service()
        {
            if (service == null)
            {
                service = new Validator(modelContext, new Parser());
            }
            return service;
        }
    }
}
