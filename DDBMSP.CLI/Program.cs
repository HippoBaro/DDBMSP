using System;
using CommandLine;
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
            return Parser.Default.ParseArguments<Generator, Populator>(args)
                .MapResult(
                    (Generator opts) => opts.Run(),
                    (Populator o) => o.Run().Result,
                    errs => 1);
        }
    }
}