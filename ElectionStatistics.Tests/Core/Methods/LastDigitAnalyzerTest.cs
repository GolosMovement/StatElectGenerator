using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using ElectionStatistics.Core.Methods;
using ElectionStatistics.Model;

namespace ElectionStatistics.Tests.Core.Methods
{
    public class LastDigitAnalyzerTest
    {
        private static LastDigitAnalyzer service;
        [Fact]
        public void Calculate_OK_ReturnsData()
        {
            var ldaResult = Service().Calculate(25,
                new List<int>() { 8, 1, 1, 1, 1, 3, 3, 3, 3, 1 });

            Assert.Equal(17.0, ldaResult.ChiSquared, 15);
            Assert.Equal(0.06, ldaResult.Sigma, 15);
            Assert.Equal(
                new List<double>() { 0.32, 0.04, 0.04, 0.04, 0.04, 0.12, 0.12, 0.12, 0.12, 0.04 },
                ldaResult.Frequency);
        }

        [Fact]
        public void Calculate_NoLineNumbers_ReturnsNull()
        {
            Assert.Null(Service().Calculate(0, null));
            Assert.Null(Service().Calculate(25, new List<int>() {}));
        }

        private LastDigitAnalyzer Service()
        {
            if (service == null)
            {
                service = new LastDigitAnalyzer();
            }

            return service;
        }
    }
}
