using System;
using System.Collections.Generic;
using Xunit;
using ElectionStatistics.Core.Preset;

namespace ElectionStatistics.Tests.Core.Preset
{
    public class ParserTest
    {
        private Parser parser = new Parser();

        [Fact]
        public void ParseDigits()
        {
            Assert.Null(parser.Execute(null));
            Assert.Empty(parser.Execute(""));
            Assert.Empty(parser.Execute(" "));
            Assert.Empty(parser.Execute(" a b c ed "));

            Assert.Equal(new [] { 30, 10 }, parser.Execute(" a b[30] c ed[10] "));
            Assert.Equal(new [] { 342, 493, 2243 },
                parser.Execute("[342]/([493]+[2243])"));
            Assert.Equal(new [] { 10, 11 }, parser.Execute("[10] / [11] * 100"));
            Assert.Equal(new [] { 3, 10, 1 }, parser.Execute(" a b[3] c ed[10] /[1] [10] "));

            Assert.Equal(new List<int>(), parser.Execute("count(1)"));
            Assert.Equal(new List<int>(), parser.Execute("sum(Value)"));
        }
    }
}
