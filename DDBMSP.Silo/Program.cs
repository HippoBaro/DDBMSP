using System;
using System.Reflection;
using System.Threading.Tasks;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Serialization;

namespace DDBMSP.Silo
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            GC.TryStartNoGCRegion(200000000);
            StartSilo();

            Console.WriteLine("Press Enter to terminate...");
            await Task.Delay(-1);
        }

        private static void StartSilo()
        {
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo();
            siloConfig.Globals.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            siloConfig.AddSimpleMessageStreamProvider("Default", true);
            siloConfig.AddMemoryStorageProvider();
            siloConfig.LoadFromFile("OrleansConfiguration.xml");
            var silo = new SiloHost("DDDMSPSilo", siloConfig); 
            silo.InitializeOrleansSilo(); 
            silo.StartOrleansSilo();
        }
    }
}