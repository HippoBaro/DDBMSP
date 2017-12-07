using System;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace DDBMSP.Silo
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = StartSilo(args);

            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();

            return exitCode;
        }

        private static int StartSilo(string[] args)
        {
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo(); 
            siloConfig.AddMemoryStorageProvider();
            var silo = new SiloHost("DDDMSPSilo", siloConfig); 
            silo.InitializeOrleansSilo(); 
            //silo.Config.Globals.RegisterDashboard();
            silo.StartOrleansSilo();

            return 0;
        }
    }
}