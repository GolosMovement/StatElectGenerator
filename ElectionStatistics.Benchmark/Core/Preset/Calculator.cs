using System;
using System.Data;
using BenchmarkDotNet.Attributes;
using NCalc;

namespace ElectionStatistics.Benchmark.Core.Preset
{
    public class Calculator
    {
        private static int rows = 200000;
        private static string formula = "{0} / ({1} + {2})";

        [Benchmark]
        public double DanglCalc()
        {
            return Calculate((x, y, z) =>
                Dangl.Calculator.Calculator.Calculate(
                    string.Format(formula, x, y, z)).Result);
        }

        [Benchmark]
        public double NCalcTwo()
        {
            return Calculate((x, y, z) =>
                (double) new Expression(
                    string.Format(formula, x, y, z)).Evaluate());
        }

        [Benchmark]
        public double DataTable()
        {
            return Calculate((x, y, z) =>
                (double) new DataTable().Compute(
                    string.Format(formula, x, y, z), null));
        }

        [Benchmark]
        public double Compiled()
        {
            return Calculate((x, y, z) => x / (y + z));
        }

        private double Calculate(Func<double, double, double, double> func)
        {
            var r = new Random();
            double lastResult = 0.0;

            for (int i = 0; i < rows; i++)
            {
                lastResult = func.Invoke(
                    (double) r.Next(1, 100),
                    (double) r.Next(1, 100),
                    (double) r.Next(1, 100));
            }

            return lastResult;
        }
    }
}
