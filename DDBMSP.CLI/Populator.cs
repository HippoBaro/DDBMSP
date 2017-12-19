using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Querier;
using DDBMSP.Interfaces.Grains.Workers;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;
using ShellProgressBar;

namespace DDBMSP.CLI
{
    [Verb("populate", HelpText = "Populate your cluster with data")]
    internal class Populator
    {

        [Option('i', "input", Required = false, HelpText = "File to populate from. Default: out.ddbmsp")]
        public string Input { get; set; }

        public List<StorageUnit> Units { get; set; } = new List<StorageUnit>();

        public async Task<int> Run() {
            Init();
            
            ReadData();
            await Upload();
            
            return 0;
        }

        private void ReadData() {
            var serializer = new JsonSerializer();

            var t = Stopwatch.StartNew();
            
            using (var s = File.Open(Input, FileMode.Open))
            using (var reader = new BsonReader(s)) {
                ReadingPB = Program.ProgressBar.Spawn((int) (s.Length/100), "Reading data...", Program.ProgressBarOption);

                reader.ReadRootValueAsArray = true;
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.StartObject) continue;
                    
                    var o = serializer.Deserialize<StorageUnit>(reader);
                    Units.Add(o);
                    
                    if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                    t.Restart();
                    ReadingPB.Tick((int) (s.Position/10));
                    if (ReadingPB.Percentage == 100)
                        ReadingPB.Tick("Closing stream...");
                }
                ReadingPB.Tick((int) (s.Length/10), "Closing stream...");
            }
            ReadingPB.Tick("Done.");
            Program.ProgressBar.Tick();
        }
        
        private static double Percentile(List<int> sequence, double excelPercentile) {
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
            
            UploadPB = Program.ProgressBar.Spawn(Units.Count, "Uploading...", Program.ProgressBarOption);
            UploadPB.Tick();
            
            var latencies = new List<int>(Units.Count);
            int ops = 0;
            int lat = 0;
            
            var t = Stopwatch.StartNew();
            for (int i = 0; i < Units.Count / Environment.ProcessorCount; i++) {
                PopulateUnit(Units.Skip(i * Environment.ProcessorCount).Take(Environment.ProcessorCount), ref ops, ref lat);
                latencies.Add(lat);
                
                if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                t.Restart();
                UploadPB.Tick(i * Environment.ProcessorCount, $"{ops*2} ops/sec — {lat}ms per batch inserts — Latency: Min = {latencies.Min()}ms, Max = {latencies.Max()}ms, Average = {latencies.Average():F3}ms, 95% = {Percentile(latencies, .95):F3}ms, 99% = {Percentile(latencies, .99):F3}ms, 99.9% = {Percentile(latencies, .999):F3}ms");
                ops = 0;
            }
            if (Units.Count % Environment.ProcessorCount != 0) {
                PopulateUnit(Units.Skip(Units.Count - Units.Count % Environment.ProcessorCount).Take(Units.Count % Environment.ProcessorCount), ref ops, ref lat);
            }
            UploadPB.Tick(Units.Count, $"Done. Latency: Min = {latencies.Min()}ms, Max = {latencies.Max()}ms, Average = {latencies.Average():F3}ms, 95% = {Percentile(latencies, .95):F3}ms, 99% = {Percentile(latencies, .99):F3}ms, 99.9% = {Percentile(latencies, .999):F3}ms");
            Program.ProgressBar.Tick(4, "Done.");
            return Task.CompletedTask;
        }

        private void PopulateUnit(IEnumerable<StorageUnit> units, ref int ops, ref int lat) {
            var tasks = new List<Task>(units.Count());
            
            for (var i = 0; i < units.Count(); i++) {
                tasks.Add(GrainClient.GrainFactory.GetGrain<IArticleDispatcher>(0)
                    .DispatchNewArticlesFromAuthor(units.ElementAt(i).User.AsImmutable(),
                        units.ElementAt(i).Articles.AsImmutable()));
            }
            var t = Stopwatch.StartNew();
            Task.WhenAll(tasks).Wait();
            lat = (int) t.ElapsedMilliseconds;
            ops += units.Sum(unit => unit.EntityCount);
        }
        
        private static void Connect() {
            var config = ClientConfiguration.LocalhostSilo();
            config.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            config.FallbackSerializationProvider = typeof(ILBasedSerializer).GetTypeInfo();
            
            try {
                InitializeWithRetries(config, 5);
            }
            catch (Exception ex) {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");
                throw;
            }
        }
        
        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing) {
            var attempt = 0;
            while (true) {
                try {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException) {
                    attempt++;
                    Console.WriteLine(
                        $"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing) {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
        
        private ChildProgressBar ReadingPB { get; set; }
        private ChildProgressBar UploadPB { get; set; }
        
        private void Init() {
            
            Connect();
            
            if (string.IsNullOrEmpty(Input)) {
                Input = Environment.CurrentDirectory + "/out.ddbmsp";
            }
            
            Program.ProgressBar = new ProgressBar(2, "Populating cluster...", Program.ProgressBarOption);
        }
    }
}