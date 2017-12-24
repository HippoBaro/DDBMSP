using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
        public static async Task Main(string[] args)
        {
            var nodes = args.Select(s => CreateIPEndPoint(s)).ToList();

            GC.TryStartNoGCRegion(200000000);
            StartSilo(nodes.FirstOrDefault());

            Console.WriteLine("Press Enter to terminate...");
            await Task.Delay(-1);
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
            var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.Globals.ClusterId = "Orleans-DDBMSP";
            config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.MembershipTableGrain;
            config.Globals.SeedNodes.Clear();
            config.Globals.SeedNodes.Add(seed ?? new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], 11111));
            config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.ReminderTableGrain;
            
            config.Defaults.PropagateActivityId = true;
            config.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, 30000);
            config.Defaults.Port = 11111;
            config.Defaults.HostNameOrIPAddress = Dns.GetHostEntry("localhost").AddressList[0].ToString();
            return config;
        }

        private static void StartSilo(IPEndPoint endpoints)
        {
            var siloConfig = GetConf(endpoints);
            siloConfig.Globals.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            siloConfig.AddSimpleMessageStreamProvider("Default", true);
            siloConfig.AddMemoryStorageProvider();
            var silo = new SiloHost(Dns.GetHostName(), siloConfig); 
            silo.InitializeOrleansSilo(); 
            silo.StartOrleansSilo();
        }
    }
}