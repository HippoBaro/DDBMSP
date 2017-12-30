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
        public const string BlogUrl = "";
        public const string BlogTitle = "DDBMSP — Project";
        public const string BlogDescription = "The fantastic world of actor-systems & random stuff";
        public const string BlogCoverImage = "https://demo.ghost.io/content/images/2017/07/blog-cover.jpg";
        public const string BlogLogo = "https://demo.ghost.io/content/images/2014/09/Ghost-Transparent-for-DARK-BG.png";

        public static string CurrentPage { get; set; }
    }
    
    public static class Program
    {
        public static async Task Main(string[] args) {
            await Task.Delay(15000);
            
            var config = new ClientConfiguration();
            if (Environment.GetEnvironmentVariable("LAUCHING_ENV") == "LOCALHOST") {
                config = ClientConfiguration.LocalhostSilo();
                config.ResponseTimeout = TimeSpan.FromMinutes(5);
            }
            else {
                var assembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
                var consulIps = Dns.GetHostAddressesAsync("consul").Result;
                config.GatewayProvider = ClientConfiguration.GatewayProviderType.Custom;
                config.ResponseTimeout = TimeSpan.FromSeconds(20);
                config.ClusterId = "DDBMSP-Cluster";
                config.CustomGatewayProviderAssemblyName = assembly;
                config.DataConnectionString = $"http://{consulIps.First()}:8500";
                config.PropagateActivityId = true;
            }
            config.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            
            InitializeWithRetries(config, 10);

            GrainClient.ClusterConnectionLost += (sender, eventArgs) => {
                InitializeWithRetries(config, -1);
            };
            
            BuildWebHost(args).Run();
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