using System;
using System.Collections.Generic;
using System.Linq;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Methods
{
    public struct LDAResult
    {
        public List<double> Frequency;
        public double ChiSquared;
        public double Sigma;
    }

    public class LastDigitAnalyzer
    {
        public LDAResult GetData(List<int> numbers)
        {
            if (numbers.Count == 0)
            {
                throw new ArgumentException("lineNumbers should not be empty");
            }

            var result = new LDAResult();
            var calcFreqs = Frequencies(numbers);
            result.ChiSquared = ChiSquared(calcFreqs, numbers);
            result.Sigma = Sigma(numbers);

            result.Frequency = calcFreqs.Select(freq => freq / numbers.Count).ToList();

            return result;
        }

        private double ChiSquared(List<double> frequencies, List<int> lineNumbers)
        {
            var expectedFrequency = lineNumbers.Count / 10.0;
            return frequencies.Aggregate(0.0, (sum, num) =>
                sum + Math.Pow(num - expectedFrequency, 2) / expectedFrequency);
        }

        private double Sigma(List<int> numbers)
        {
            return Math.Sqrt(0.1 * 0.9 / numbers.Count);
        }

        private List<double> Frequencies(List<int> numbers)
        {
            var freqs = new List<double>(new double[10]);
            foreach (int number in numbers)
            {
                ++freqs[LastDigit(number)];
            }

            return freqs;
        }

        private int LastDigit(int number)
        {
            return number % 10;
        }
    }
}
