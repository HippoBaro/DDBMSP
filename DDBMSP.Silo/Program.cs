using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Serialization;

namespace DDBMSP.Silo
{
    public static class Program
    {
        public static ISiloHost SiloHost { get; set; }
        
        public static async Task Main() {
            await Task.Delay(5000);
            StartSilo();

            Console.WriteLine("Silo up");
            await Task.Delay(-1);
        }

        private static void StartSilo() {
            
            
            var config = new SiloHost(Dns.GetHostName(), new FileInfo("OrleansConfiguration.xml"));

            if (Environment.GetEnvironmentVariable("LAUCHING_ENV") == "LOCALHOST") {
                config.Config = ClusterConfiguration.LocalhostPrimarySilo();
                config.Config.AddMemoryStorageProvider("RedisStore");
            }
            else {
                config.Config.Globals.MembershipTableAssembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
                var consulIps = Dns.GetHostAddressesAsync("consul").Result;
                config.Config.Globals.DataConnectionString = $"http://{consulIps.First()}:8500";
                config.Config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.Custom;
                
                var ips = Dns.GetHostAddressesAsync(Dns.GetHostName()).Result;
                config.Config.Defaults.HostNameOrIPAddress = ips.FirstOrDefault()?.ToString();
                
                config.Config.Globals.RegisterStorageProvider<Orleans.StorageProviders.RedisStorage.RedisStorage>(
                    "RedisStore", new Dictionary<string, string>() {
                        { "RedisConnectionString", "storage" },
                        { "UseJsonFormat", "false" }
                    });
            }
            
            
            config.Config.Globals.ClusterId = "DDBMSP-Cluster";
            config.Config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.Disabled;
            config.Config.Globals.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            config.Config.RegisterDashboard();

            var builder = SiloHostBuilder.CreateDefault().UseConfiguration(config.Config);

            builder.GetApplicationPartManager()
                .AddApplicationPart(typeof(Common.CSharpRepl.Context).Assembly)
                .AddApplicationPart(typeof(ArticleState).Assembly)
                .AddFromApplicationBaseDirectory()
                .AddApplicationPart(typeof(Interfaces.Grains.Aggregators.IAggregator<>).Assembly);

            SiloHost = builder.ConfigureSiloName(Dns.GetHostName())
                .UseDashboard(options => {
                    options.HostSelf = true;
                    options.Port = 8080;
                    options.Host = "*";
                })
                .Build();

            SiloHost.StartAsync();
        }
    }
}