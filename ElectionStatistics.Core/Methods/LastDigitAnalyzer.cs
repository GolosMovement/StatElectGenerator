using System;
using System.Collections.Generic;
using System.Linq;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Methods
{
    public class LDAResult
    {
        public List<double> Frequency { get; set; }
        public double ChiSquared { get; set; }
        public double Sigma { get; set; }
    }

    public class LastDigitAnalyzer
    {
        public LDAResult Calculate(int total, List<int> numbers)
        {
            if (total == 0 || numbers == null || numbers.Count == 0)
            {
                return null;
            }

            LDAResult result = new LDAResult();

            result.Sigma = Math.Sqrt(0.1 * 0.9 / total);

            var expectedFrequency = total / 10.0;
            result.ChiSquared = numbers.Aggregate(0.0, (sum, num) =>
                sum + Math.Pow(num - expectedFrequency, 2) / expectedFrequency);

            result.Frequency = new List<double>();
            for (int i = 0; i < numbers.Count; i++)
            {
                result.Frequency.Add(numbers[i] / (double) total);
            }

            return result;
        }
    }
}
