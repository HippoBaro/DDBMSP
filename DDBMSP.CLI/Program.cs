using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.CLI.Benchmark;
using DDBMSP.Entities.Article;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;
using ShellProgressBar;

namespace DDBMSP.CLI
{
    internal static class Program
    {
        public static ProgressBar ProgressBar { get; set; }
        public static int ProgressBarRefreshDelay { get; set; } = 1000;

        public static ProgressBarOptions ProgressBarOption { get; } = new ProgressBarOptions {
            CollapseWhenFinished = false,
            ProgressBarOnBottom = true,
            ForegroundColor = ConsoleColor.Blue,
            ForegroundColorDone = ConsoleColor.Green,
            ProgressCharacter = '—'
        };
        
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