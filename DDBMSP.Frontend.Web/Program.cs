using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;

namespace DDBMSP.Frontend.Web
{
    public static class Constants
    {
        public const string BlogUrl = "http://localhost:5000";
        public const string BlogTitle = "DDBMSP — Project";
        public const string BlogDescription = "description";
        public const string BlogCoverImage = "https://demo.ghost.io/content/images/2017/07/blog-cover.jpg";
        public const string BlogLogo = "https://demo.ghost.io/content/images/2014/09/Ghost-Transparent-for-DARK-BG.png";

        public static string CurrentPage { get; set; }
    }
    
    public static class Program
    {
        public static Task Main(string[] args) {
            var assembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
            var consulIps = Dns.GetHostAddressesAsync("consul").Result;
            
            var config = new ClientConfiguration {
                GatewayProvider = ClientConfiguration.GatewayProviderType.Custom,
                ResponseTimeout = TimeSpan.FromSeconds(5),
                ClusterId = "DDBMSP-Cluster",
                CustomGatewayProviderAssemblyName = assembly,
                DataConnectionString = $"http://{consulIps.First()}:8500",
                PropagateActivityId = true
            };
            
            config.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            
            InitializeWithRetries(config, 10);

            GrainClient.ClusterConnectionLost += (sender, eventArgs) => {
                InitializeWithRetries(config, -1);
            };
            
            BuildWebHost(args).Run();
            return Task.CompletedTask;
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .Build();
        
        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            var attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (Exception)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing && initializeAttemptsBeforeFailing != -1)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
        }
    }
    
    
}