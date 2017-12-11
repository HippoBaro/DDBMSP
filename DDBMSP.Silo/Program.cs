using System;
using System.Reflection;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Serialization;

namespace DDBMSP.Silo
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            GC.TryStartNoGCRegion(200000000);
            var exitCode = StartSilo(args);

            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();

            return exitCode;
        }

        private static int StartSilo(string[] args)
        {
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo();
            siloConfig.Globals.FallbackSerializationProvider = typeof(ILBasedSerializer).GetTypeInfo();
            siloConfig.AddSimpleMessageStreamProvider("Default", true);
            siloConfig.AddMemoryStorageProvider();
            siloConfig.LoadFromFile("OrleansConfiguration.xml");
            var silo = new SiloHost("DDDMSPSilo", siloConfig); 
            silo.InitializeOrleansSilo(); 
            //silo.Config.Globals.RegisterDashboard();
            silo.StartOrleansSilo();
            
            return 0;
        }
    }
}