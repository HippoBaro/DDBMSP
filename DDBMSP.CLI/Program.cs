using System;
using CommandLine;
using DDBMSP.CLI.Benchmark;

namespace DDBMSP.CLI
{
    internal static class Program
    {
        public static int ProgressBarRefreshDelay { get; set; } = 1000;
        
        private static int Main(string[] args) {
            return Parser.Default.ParseArguments<Generator, Populator, Interactive.Interactive, StorageStats, Benchmarker>(args)
                .MapResult(
                    (Generator opts) => opts.Run(),
                    (Populator o) => o.Run().Result,
                    (Interactive.Interactive o) => o.Run().Result,
                    (StorageStats o) => o.Run().Result,
                    (Benchmarker o) => o.Run().Result,
                    errs => 1);
        }
    }
}