using System;
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
        }
    }
}
