using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.CLI.Core;
using DDBMSP.Entities;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;

namespace DDBMSP.CLI
{
    [Verb("stats", HelpText = "Get statistics on storage")]
    internal class StorageStats : ConnectedTool
    {

        [Option('i', "input", Required = false, HelpText = "File to populate from. Default: out.ddbmsp")]
        public string Input { get; set; }

        public List<StorageUnit> Units { get; set; } = new List<StorageUnit>();

        public async Task<int> Run() {
            Init();
            
            await QueryAll();
            
            return 0;
        }

        private async Task QueryAll() {
            Console.WriteLine("User:");
            await Query<UserState>();
            Console.WriteLine();
            
            Console.WriteLine("Articles:");
            await Query<ArticleState>();
            Console.WriteLine();
            
            Console.WriteLine("Activities:");
            await Query<List<UserActivityState>>();
            Console.WriteLine();
        }
        
        private async Task Query<TRessource>() {
            var hash = ClusterClient.GetGrain<IDistributedHashTable<Guid, TRessource>>(0);
            var stats = await hash.GetBucketUsage();
            
            Console.WriteLine($"\tTotal: {stats.Sum()}");
            Console.WriteLine($"\tAverage: {stats.Average()}");
            Console.WriteLine($"\tMin: {stats.Min()}");
            Console.WriteLine($"\tMax: {stats.Max()}");
            Console.WriteLine($"\tDelta: {stats.Max() - stats.Min()}");
        }
        
        private void Init() {
            if (string.IsNullOrEmpty(Input)) {
                Input = Environment.CurrentDirectory + "/out.ddbmsp";
            }
        }
    }
}