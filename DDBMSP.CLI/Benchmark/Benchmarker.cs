using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.CLI.Core;
using DDBMSP.Entities;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.CLI.Benchmark
{
    public class BenchmarckerInstance
    {
        private IClusterClient Client { get; }
        private TimeSpan TimeToLive { get; }
        private List<StorageUnit> Units { get; }

        public BenchmarckerInstance(IClusterClient client, TimeSpan timeToLive, List<StorageUnit> units) {
            Client = client;
            TimeToLive = timeToLive.Add(TimeSpan.FromSeconds(1));
            Units = units;
        }

        public async Task<List<long>> Run() {
            var articles = Client.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var activities = Client.GetGrain<IDistributedHashTable<Guid, List<UserActivityState>>>(0);
            var user = Client.GetGrain<IDistributedHashTable<Guid, UserState>>(0);
            
            var latencies = new List<long>(100000);
            
            var t = Stopwatch.StartNew();
            while (t.Elapsed < TimeToLive) {
                var unit = Units[RandomGenerationData.Random.Next(Units.Count)];
                var lat = Stopwatch.StartNew();
                switch (RandomGenerationData.Random.Next(3)) {
                    case 0:
                    {
                        var guid = unit.Articles[RandomGenerationData.Random.Next(unit.Articles.Count)].Id;
                        try {
                            
                            await articles.Get(guid.AsImmutable());
                        }
                        catch (Exception e) {
                            Console.WriteLine($"Error upon access to ressource {guid} (ArticleState)");
                            Console.WriteLine(e);
                            throw;
                        }
                        
                        break;
                    }
                    case 1:
                    {
                        var guid = unit.User.Id;
                        try {
                            await user.Get(guid.AsImmutable());
                        }
                        catch (Exception e) {
                            Console.WriteLine($"Error upon access to ressource {guid} (UserState)");
                            Console.WriteLine(e);
                            throw;
                        }
                        
                        break;
                    }
                    case 2:
                    {
                        var guid = unit.Articles[RandomGenerationData.Random.Next(unit.Articles.Count)].Id;
                        try {
                            await activities.Get(guid.AsImmutable());
                        }
                        catch (Exception e) {
                            Console.WriteLine($"Error upon access to ressource {guid} (Activities)");
                            Console.WriteLine(e);
                            throw;
                        }
                        break;
                    }
                    default:
                        throw new IndexOutOfRangeException();
                }
                if (t.ElapsedMilliseconds > 1000)
                    latencies.Add(lat.ElapsedMilliseconds);
            }
            return latencies;
        }
    }

    [Verb("benchmark", HelpText = "Benchmark a cluster")]
    public class Benchmarker : ConnectedTool
    {
        [Option('t', "time", Required = true, HelpText = "Time to benchmark the cluster in seconds")]
        public int TimeToLiveInSeconds { get; set; }

        [Option('j', "jobs", Required = true, HelpText = "Concurrent benchmarking jobs to spawn")]
        public int TaskTargetCount { get; set; }
        
        [Option('i', "input", Required = false, HelpText = "File to benckmark with. Default: out.ddbmsp")]
        public string Input { get; set; }

        private List<BenchmarckerInstance> Benchmarkers { get; set; }
        private List<StorageUnit> Units { get; } = new List<StorageUnit>();

        public async Task<int> Run() {
            Benchmarkers = new List<BenchmarckerInstance>(TaskTargetCount);
            
            ReadData();

            Console.WriteLine($"Benchmarking for {TimeToLiveInSeconds} seconds...");
            for (var i = 0; i < TaskTargetCount; i++) {
                var client = await ConnectClient();
                Benchmarkers.Add(new BenchmarckerInstance(client, TimeSpan.FromSeconds(TimeToLiveInSeconds), Units));
            }

            var lats = await Task.WhenAll(Benchmarkers.Select(benchmarker => benchmarker.Run()).ToList());

            var lat = lats.SelectMany(list => list).ToList();
            
            Console.WriteLine("Report:");
            
            Console.WriteLine($"All job");
            Console.WriteLine($"{lat.Count} items retreived");
            Console.WriteLine("Latencies:");
            Console.WriteLine($"\tMin: {lat.Min()}ms");
            Console.WriteLine($"\tMax: {lat.Max()}ms");
            Console.WriteLine($"\tAverage: {lat.Average()}ms");
            Console.WriteLine($"\t95%: {Percentile(lat, .95)}ms");
            Console.WriteLine($"\t99%: {Percentile(lat, .99)}ms");
            Console.WriteLine($"\t99.9%: {Percentile(lat, .999)}ms");

            int job = 0;
            foreach (var list in lats) {
                Console.WriteLine($"\nJob #{job++}");
                Console.WriteLine($"{list.Count} items retreived");
                Console.WriteLine("Latencies:");
                Console.WriteLine($"\tMin: {list.Min()}ms");
                Console.WriteLine($"\tMax: {list.Max()}ms");
                Console.WriteLine($"\tAverage: {list.Average()}ms");
                Console.WriteLine($"\t95%: {Percentile(list, .95)}ms");
                Console.WriteLine($"\t99%: {Percentile(list, .99)}ms");
                Console.WriteLine($"\t99.9%: {Percentile(list, .999)}ms");
            }

            return 0;
        }
        
        private static double Percentile(List<long> sequence, double excelPercentile) {
            sequence.Sort();
            var N = sequence.Count;
            var n = (N - 1) * excelPercentile + 1;
            if (n == 1d) return sequence[0];
            if (n == N) return sequence[N - 1];

            var k = (int) n;
            var d = n - k;
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }
        
        private void ReadData() {
            if (string.IsNullOrEmpty(Input)) {
                Input = "/exportcli" + "/out.ddbmsp";
            }
            
            Console.WriteLine("Reading input...");
            var serializer = new JsonSerializer();
           
            using (var s = File.Open(Input, FileMode.Open))
            using (var reader = new BsonReader(s)) {
                reader.ReadRootValueAsArray = true;
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.StartObject) continue;
                    
                    var o = serializer.Deserialize<StorageUnit>(reader);
                    Units.Add(o);
                }
            }
            Console.WriteLine("Reading input... Done.");
        }
    }
}