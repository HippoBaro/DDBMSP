using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Serialization;

namespace DDBMSP.Silo
{
    public static class Program
    {
        public static SiloHost SiloHost { get; set; }
        
        public static async Task Main()
        {
            GC.TryStartNoGCRegion(200000000);
            StartSilo();

            Console.WriteLine("Silo up");
            await Task.Delay(-1);
        }

        private static void StartSilo()
        {
            SiloHost = new SiloHost(Dns.GetHostName(), new FileInfo("OrleansConfiguration.xml"));
            
            SiloHost.Config.Globals.MembershipTableAssembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
            
            var assembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
            var consulIps = Dns.GetHostAddressesAsync("consul").Result;
            
            SiloHost.Config.Globals.DataConnectionString = $"http://{consulIps.First()}:8500";
            SiloHost.Config.Globals.ClusterId = "DDBMSP-Cluster";
            SiloHost.Config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.Custom;
            SiloHost.Config.Globals.MembershipTableAssembly = assembly;
            SiloHost.Config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.Disabled;
            
            var ips = Dns.GetHostAddressesAsync(Dns.GetHostName()).Result;
            SiloHost.Config.Defaults.HostNameOrIPAddress = ips.FirstOrDefault()?.ToString();
            
            SiloHost.Config.Globals.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());

            SiloHost.Config.Globals.RegisterStorageProvider<Orleans.StorageProviders.RedisStorage.RedisStorage>(
                "RedisStore", new Dictionary<string, string>() {
                    {"RedisConnectionString", "storage"}
                });

            SiloHost.InitializeOrleansSilo();
            SiloHost.StartOrleansSilo(false);
        }
    }
}