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
        public void GetData_OK_ReturnsData()
        {
            var minValue = 10;

            var numbers = new List<int> { 10, 10, 10, 11, 13, 14, 15, 15, 17, 18 };
            LDAResult result = Service().GetData(numbers, minValue);
            Assert.Equal(Math.Sqrt(0.09/numbers.Count), result.Sigma, 15);
            Assert.Equal(8, result.ChiSquared);
            Assert.Equal(new List<double>() { 0.3, 0.1, 0.0, 0.1, 0.1, 0.2, 0.0, 0.1, 0.1, 0.0 },
                result.Frequency);
        }

        [Fact]
        public void GetData_NoLineNumbers_ThrowsException()
        {
            Assert.Throws<ArgumentException>(
                () => Service().GetData(new List<int>(), 0));
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
