using System;
using System.Threading.Tasks;
using DDBMSP.Grains.Contracts;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace DDBMSP
{
    class Program
    {
        static async Task RunTest()
        {
            
            var clientConfig = ClientConfiguration.LocalhostSilo(); 
            var client = new ClientBuilder().UseConfiguration(clientConfig).Build();
            
            await client.Connect();
            
            var user = client.GetGrain<IUserGrain>(42);
            System.Console.WriteLine(await user.Walk());
        }
        
        static void Main(string[] args)
        {
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo(); 
            var silo = new SiloHost("Test Silo", siloConfig);
            silo.InitializeOrleansSilo(); 
            silo.StartOrleansSilo();
            
            RunTest().Wait();
            
            silo.ShutdownOrleansSilo();
        }
    }
}