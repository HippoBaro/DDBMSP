using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
        private IClusterClient _clusterClient;

        protected IClusterClient ClusterClient => _clusterClient ?? (_clusterClient = ConnectClient().Result);

        public ConnectedTool() {}
        protected ConnectedTool(IClusterClient client) => _clusterClient = client;

        protected Task<IClusterClient> ConnectClient() {
            var assembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
            var consulIps = Dns.GetHostAddressesAsync("consul").Result;
            
            var config = new ClientConfiguration {
                GatewayProvider = ClientConfiguration.GatewayProviderType.Custom,
                ResponseTimeout = TimeSpan.FromMinutes(5),
                ClusterId = "DDBMSP-Cluster",
                CustomGatewayProviderAssemblyName = assembly,
                DataConnectionString = $"http://{consulIps.First()}:8500",
                PropagateActivityId = true
            };

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