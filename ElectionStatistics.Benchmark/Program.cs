using System;
using System.Data;
using BenchmarkDotNet.Running;

namespace ElectionStatistics.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Core.Preset.Calculator>();
        }
    }
}
