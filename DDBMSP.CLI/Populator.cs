using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.CLI.Core;
using DDBMSP.Entities;
using DDBMSP.Interfaces.Grains.Workers;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Orleans.Concurrency;

namespace DDBMSP.CLI
{
    [Verb("populate", HelpText = "Populate your cluster with data")]
    internal class Populator : ConnectedTool
    {
        [Option('i', "input", Required = false, HelpText = "File to populate from. Default: out.ddbmsp")]
        public string Input { get; set; }

        public List<StorageUnit> Units { get; set; }

        public int BytesPerUnit { get; set; }

        public async Task<int> Run() {
            Init();
            
            Console.WriteLine("Reading data...\n");
            ReadData();
            Console.WriteLine("Reading data... Done.");
            
            Console.WriteLine("Uploading data...");
            await Upload();
            
            return 0;
        }

        private void ReadData() {
            var serializer = new JsonSerializer();
            
            using (var s = File.Open(Input, FileMode.Open))
            using (var reader = new BsonReader(s)) {
                Units = serializer.Deserialize<List<StorageUnit>>(reader);
            }
        }
        
        private static double Percentile(List<float> sequence, double excelPercentile) {
            sequence.Sort();
            var N = sequence.Count;
            var n = (N - 1) * excelPercentile + 1;
            if (n == 1d) return sequence[0];
            if (n == N) return sequence[N - 1];

            var k = (int) n;
            var d = n - k;
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }

        private Task Upload() {
            var latencies = new List<float>(Units.Count);
            var ops = 0;
            float lat = 0;
            var unit = 0;
            
            var t = Stopwatch.StartNew();
            for (var i = 0; i < Units.Count / Environment.ProcessorCount * Environment.ProcessorCount; i++) {
                PopulateUnit(Units.Skip(i * Environment.ProcessorCount * Environment.ProcessorCount).Take(Environment.ProcessorCount * Environment.ProcessorCount), ref ops, ref lat, ref unit);
                latencies.Add(lat);
                
                if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                t.Restart();
                Console.WriteLine($"Uploading... {ops*2} ops/sec, {unit * BytesPerUnit / 1000000}MB/s — {lat}ms per batch inserts — Latency: Min = {latencies.Min()}ms, Max = {latencies.Max()}ms, Average = {latencies.Average():F3}ms, 95% = {Percentile(latencies, .95):F3}ms, 99% = {Percentile(latencies, .99):F3}ms, 99.9% = {Percentile(latencies, .999):F3}ms\r");
                ops = 0;
                unit = 0;
            }
            if (Units.Count % Environment.ProcessorCount * Environment.ProcessorCount != 0) {
                PopulateUnit(Units.Skip(Units.Count - Units.Count % Environment.ProcessorCount * Environment.ProcessorCount).Take(Units.Count % Environment.ProcessorCount * Environment.ProcessorCount), ref ops, ref lat, ref unit);
            }
            Console.WriteLine($"Done. Latency: Min = {latencies.Min()}ms, Max = {latencies.Max()}ms, Average = {latencies.Average():F3}ms, 95% = {Percentile(latencies, .95):F3}ms, 99% = {Percentile(latencies, .99):F3}ms, 99.9% = {Percentile(latencies, .999):F3}ms");
            return Task.CompletedTask;
        }

        private void PopulateUnit(IEnumerable<StorageUnit> units, ref int ops, ref float lat, ref int unit) {
            var tasks = new List<Task>(units.Count());
            
            for (var i = 0; i < units.Count() / Environment.ProcessorCount; i++) {
                tasks.Add(ClusterClient.GetGrain<IArticleDispatcherWorker>(0)
                    .DispatchStorageUnits(units.Skip(i * Environment.ProcessorCount).Take(Environment.ProcessorCount).ToList().AsImmutable()));
            }
            var t = Stopwatch.StartNew();
            Task.WhenAll(tasks).Wait();
            lat = t.ElapsedMilliseconds;
            ops += units.Sum(u => u.EntityCount);
            unit += units.Count();
        }
        
        private void Init() {
            
            if (string.IsNullOrEmpty(Input)) {
                Input = Environment.CurrentDirectory + "/out.ddbmsp";
            }
        }
    }
}