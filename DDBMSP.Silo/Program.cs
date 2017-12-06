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

            //either StartSilo or ShutdownSilo failed would result on a non-zero exit code. 
            return exitCode;
        }


        private static int StartSilo(string[] args)
        {
            // define the cluster configuration
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo(); 
            siloConfig.AddMemoryStorageProvider();
            var silo = new SiloHost("DDDMSPSilo", siloConfig); 
            silo.InitializeOrleansSilo(); 
            silo.StartOrleansSilo();

            return 0;
        }
    }
}