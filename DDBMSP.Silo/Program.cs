using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Lucene.Net.Support;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Serialization;

namespace DDBMSP.Silo
{
    public static class Program
    {
        public static SiloHost SiloHost { get; set; }
        
        public static async Task Main(string[] args)
        {
            var nodes = args.Select(s => CreateIPEndPoint(s)).ToList();

            GC.TryStartNoGCRegion(200000000);
            StartSilo(nodes.FirstOrDefault());

            Console.WriteLine("Type 'quit' to exit");
            while (!Console.ReadLine().Contains("quit")) ;
            Console.WriteLine("Exiting");
            
            SiloHost.ShutdownOrleansSilo();
            SiloHost.StopOrleansSilo();
            
        }
        
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if(ep.Length != 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if(!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
            int port;
            if(!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }

        private static ClusterConfiguration GetConf(IPEndPoint seed) {
            var config = new ClusterConfiguration();
            config.CreateNodeConfigurationForSilo("siloTest");
            config.Globals.ClusterId = "Orleans-DDBMSP";
            config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.MembershipTableGrain;
            config.Globals.SeedNodes.Clear();
            config.Globals.SeedNodes.Add(seed ?? new IPEndPoint(IPAddress.Parse("192.168.50.38"), 11111));
            config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.ReminderTableGrain;
            
            config.Defaults.PropagateActivityId = true;
            config.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, 30000);
            config.Defaults.Port = 11111;
            config.Defaults.HostNameOrIPAddress = Dns.GetHostEntry("localhost").AddressList[0].ToString();
            return config;
        }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private static void StartSilo(IPEndPoint endpoint)
        {
            /*var siloConfig = GetConf(endpoints);
            siloConfig.Globals.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            siloConfig.AddSimpleMessageStreamProvider("Default", true);
            siloConfig.AddMemoryStorageProvider();
            SiloHost = new SiloHost(Dns.GetHostName(), siloConfig); 
            SiloHost.LoadOrleansConfig();*/
            
            SiloHost = new SiloHost(Dns.GetHostName(), new FileInfo("OrleansConfiguration.xml"));
            SiloHost.Config.AddMemoryStorageProvider();
            
            //SiloHost.Config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.Custom;
            SiloHost.Config.Globals.MembershipTableAssembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
            //SiloHost.Config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.Disabled;
            
            var assembly = typeof(Orleans.ConsulUtils.LegacyConsulGatewayListProviderConfigurator).Assembly.FullName;
            
            var consulIps = Dns.GetHostAddressesAsync("consul").Result;
            SiloHost.Config.Globals.DataConnectionString = $"http://{consulIps.First()}:8500";
            SiloHost.Config.Globals.DeploymentId = "OrleansPlayground";
            SiloHost.Config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.Custom;
            SiloHost.Config.Globals.MembershipTableAssembly = assembly; //"OrleansConsulUtils";
            SiloHost.Config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.Disabled;
            
            var ips = Dns.GetHostAddressesAsync(Dns.GetHostName()).Result;
            SiloHost.Config.Defaults.HostNameOrIPAddress = ips.FirstOrDefault()?.ToString();
            
            //192.168.50.243:11111
            //
            // The Cluster config is quirky and weird to configure in code, so we're going to use a config file


            //SiloHost.Config.Defaults.ProxyGatewayEndpoint = CreateIPEndPoint($"{GetLocalIPAddress()}:11111");
            //Si
            

            SiloHost.InitializeOrleansSilo();
            
            SiloHost.StartOrleansSilo(false);
        }
    }
}