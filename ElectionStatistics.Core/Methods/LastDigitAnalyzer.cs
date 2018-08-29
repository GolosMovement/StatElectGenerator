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
        public LDAResult GetData(List<LineNumber> lineNumbers, int? minValue)
        {
            if (minValue != null)
            {
                lineNumbers = lineNumbers.Where((x) => x.Value >= minValue).ToList();
            }

            if (lineNumbers.Count == 0)
            {
                throw new ArgumentException("lineNumbers should not be empty");
            }

            var result = new LDAResult();
            var calcFreqs = Frequencies(lineNumbers);
            result.ChiSquared = ChiSquared(calcFreqs, lineNumbers);
            result.Sigma = Sigma(lineNumbers);

            result.Frequency = calcFreqs.Select(freq => freq / lineNumbers.Count).ToList();

            return result;
        }

        private double ChiSquared(List<double> frequencies, List<LineNumber> lineNumbers)
        {
            var expectedFrequency = lineNumbers.Count / 10.0;
            return frequencies.Aggregate(0.0, (sum, num) =>
                sum + Math.Pow(num - expectedFrequency, 2) / expectedFrequency);
        }

        private double Sigma(List<LineNumber> lineNumbers)
        {
            return Math.Sqrt(0.1 * 0.9 / lineNumbers.Count);
        }

        private List<double> Frequencies(List<LineNumber> lineNumbers)
        {
            var freqs = new List<double>(new double[10]);
            foreach (LineNumber lineNumber in lineNumbers)
            {
                ++freqs[LastDigit(lineNumber)];
            }

            return freqs;
        }

        private int LastDigit(LineNumber lineNumber)
        {
            return lineNumber.Value % 10;
        }
    }
}
