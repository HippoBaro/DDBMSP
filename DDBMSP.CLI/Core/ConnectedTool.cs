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

namespace DDBMSP.CLI.Core
{
    public class ConnectedTool
    {
        [Option('z', "zzzz", Required = true, HelpText = "File to populate from. Default: out.ddbmsp")]
        public string Input { get; set; }
        
        protected IClusterClient ClusterClient { get; }

        protected ConnectedTool() => ClusterClient = ConnectClient().Result;

        protected Task<IClusterClient> ConnectClient() {
            var config = ClientConfiguration.LocalhostSilo();
            config.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());

            try {
                return InitializeWithRetries(config, 5);
            }
            catch (Exception ex) {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");
                throw;
            }
        }

        private async Task<IClusterClient> InitializeWithRetries(ClientConfiguration config,
            int initializeAttemptsBeforeFailing)
        {
            var attempt = 0;
            while (true) {
                try {
                    var client = new ClientBuilder();
                    client.GetApplicationPartManager()
                        .AddApplicationPart(typeof(Benchmarker).Assembly)
                        .AddApplicationPart(typeof(Common.CSharpRepl.Context).Assembly)
                        .AddApplicationPart(typeof(ArticleState).Assembly)
                        .AddApplicationPart(typeof(Interfaces.Grains.Aggregators.IAggregator<>).Assembly);
                    client.UseConfiguration(config);
                    
                    var ret = client.Build();
                    await ret.Connect();
                    return ret;
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