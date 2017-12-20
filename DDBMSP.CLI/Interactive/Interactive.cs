using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using DDBMSP.Common;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;

namespace DDBMSP.CLI.Interactive
{
    [Verb("interact", HelpText = "Run queries against your cluster")]
    public class Interactive
    {
        public CSharpRepl Repl { get; set; } = new CSharpRepl();
        
        [Verb("quit", HelpText = "Exit the CLI interactive mode")]
        public class Quit
        {
            public int Run() {
                Environment.Exit(0);
                return 0;
            }
        }

        public async Task<int> Run() {
            Connect();
            
            var parser = new Parser(settings => {
                settings.IgnoreUnknownArguments = true;
                settings.HelpWriter = null;
                settings.CaseSensitive = false;
            });
            
            while (true) {
                bool shouldInterpret = true;
                var line = ReadLine.Read("127.0.0.1> ");
                if (string.IsNullOrWhiteSpace(line)) {
                    Console.ReadKey(true);
                    continue;
                }
                ReadLine.AddHistory(line);
                
                var result = parser.ParseArguments<CommitQuery, ExecuteQuery, Quit>(SplitCommandLine(line));
                result.WithNotParsed(errors => {
                    if (errors.Any(error => error.Tag == ErrorType.BadVerbSelectedError)) return;
                    var message = HelpText.AutoBuild(result);
                    message.Copyright = "";
                    message.Heading = "";
                    Console.WriteLine(message.ToString());
                    shouldInterpret = false;
                });
                result.WithParsed(o => shouldInterpret = false);
                
                var ret = result
                    .MapResult(
                        (CommitQuery opts) => opts.Run().Result,
                        (ExecuteQuery o) => o.Run(Repl).Result,
                        (Quit o) => o.Run(),
                        errs => -1);

                if (shouldInterpret) {
                    try {
                        var res = await Repl.REPL(line);
                        if (res != null)
                            Console.WriteLine(res);
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
                
                Console.ReadKey(true);
            }
        }
        
        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            var inQuotes = false;
            return commandLine.Split(c =>
                {
                    if (c == '\"')
                        inQuotes = !inQuotes;

                    return !inQuotes && c == ' ';
                })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }
        
        private static void Connect() {
            var config = ClientConfiguration.LocalhostSilo();
            config.ResponseTimeout = TimeSpan.FromMinutes(5);
            //config.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            //config.FallbackSerializationProvider = typeof(ILBasedSerializer).GetTypeInfo();
            
            try {
                InitializeWithRetries(config, 5);
            }
            catch (Exception ex) {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");
                throw;
            }
        }
        
        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing) {
            var attempt = 0;
            while (true) {
                try {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException) {
                    attempt++;
                    Console.WriteLine(
                        $"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing) {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
}