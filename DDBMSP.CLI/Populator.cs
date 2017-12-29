using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.CLI.Core;
using DDBMSP.Entities;
using DDBMSP.Interfaces.Grains.Workers;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Orleans;
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
            
            Console.Write("Reading data...\r");
            ReadData();
            Console.WriteLine("Reading data... Done.") ;
            
            await Upload();
            Console.WriteLine("\nUploading data... Done.");
            
            Environment.Exit(0);
            return 0;
        }

        private void ReadData() {
            var serializer = new JsonSerializer();
            
            using (var s = File.Open(Input, FileMode.Open))
            using (var reader = new BsonReader(s)) {
                reader.ReadRootValueAsArray = true;
                Units = serializer.Deserialize<List<StorageUnit>>(reader);
                BytesPerUnit = (int) (s.Length / Units.Count);
            }
        }
        
        private static double Percentile(IEnumerable<double> sequence, double excelPercentile) {
            sequence = sequence.OrderBy(d1 => d1);
            var N = sequence.Count();
            var n = (N - 1) * excelPercentile + 1;
            if (n == 1d) return sequence.ElementAt(0);
            if (n == N) return sequence.ElementAt(N - 1);

            var k = (int) n;
            var d = n - k;
            return sequence.ElementAt(k - 1) + d * (sequence.ElementAt(k) - sequence.ElementAt(k - 1));
        }

        private static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize) {
            for (var i = 0; i < locations.Count; i += nSize) {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        private async Task Upload() {
            var sublists = SplitList(Units, Units.Count / Environment.ProcessorCount).ToList();
            var tasks = new List<Task>(sublists.Count);

            _lat999 = new double[sublists.Count];
            _lat99 = new double[sublists.Count];
            _lat95 = new double[sublists.Count];
            _latmax = new double[sublists.Count];
            _latmin = new double[sublists.Count];
            _latav = new double[sublists.Count];
            _lat = new double[sublists.Count];
            _tunit = new int[sublists.Count];

            var ops = new List<int>();
            var tp = new List<int>();
            var report = new Thread(() => {
                var t = Stopwatch.StartNew();
                while (true) {
                    Thread.Sleep(1000);
                    ops.Add(_ops);
                    tp.Add(_unit * BytesPerUnit / 1000000);
                    
                    Console.Write($"Uploading... [{t.Elapsed.Seconds:00}s] {(int)ops.Average()} op/s, {(int)tp.Average()} MB/s — Latency: Min = {_latmin.Min()}ms, Max = {_latmax.Max()}ms, Average = {_latav.Average():F3}ms, 95% = {Percentile(_lat95, .95):F3}ms, 99% = {Percentile(_lat99, .99):F3}ms, 99.9% = {Percentile(_lat999, .999):F3}ms      \r");
                    _ops = 0;
                    _unit = 0;
                }
            }) {
                Priority = ThreadPriority.Highest
            };

            var clients = new List<IClusterClient>();
            Console.Write("Connecting Clients...\r");
            for (var i = 0; i < sublists.Count; i++) {
                clients.Add(await ConnectClient());
            }
            Console.WriteLine("Connecting Clients... Done.");
            
            Console.Write("Uploading data...\r");
            report.Start();
            tasks.AddRange(sublists.Select((t, i) => Upload(t, i, clients[i])));

            await Task.WhenAll(tasks);
        }

        private int _ops;
        private int _unit;

        private double[] _lat999;
        private double[] _lat99;
        private double[] _lat95;
        private double[] _latmax;
        private double[] _latmin;
        private double[] _latav;

        private async Task Upload(IReadOnlyCollection<StorageUnit> unitsSubset, int id, IGrainFactory client) {
            var latenciesLocal = new List<double>(unitsSubset.Count);
            
            for (var i = 0; i < unitsSubset.Count / Environment.ProcessorCount; i++) {
                var lat = await PopulateUnit(unitsSubset.Skip(i * Environment.ProcessorCount).Take(Environment.ProcessorCount), id, client);
                latenciesLocal.Add(lat);
                
                _lat999[id] = Percentile(latenciesLocal, .999);
                _lat99[id] = Percentile(latenciesLocal, .99);
                _lat95[id] = Percentile(latenciesLocal, .95);
                _latmax[id] = latenciesLocal.Max();
                _latmin[id] = latenciesLocal.Min();
                _latav[id] = latenciesLocal.Average();
            }

            if (_tunit[id] >= unitsSubset.Count) return;
            
            await PopulateUnit(unitsSubset.Skip(_tunit[id]).Take(unitsSubset.Count - _tunit[id]), id, client);
            latenciesLocal.Add(_lat[id]);
            _lat999[id] = Percentile(latenciesLocal, .999);
            _lat99[id] = Percentile(latenciesLocal, .99);
            _lat95[id] = Percentile(latenciesLocal, .95);
            _latmax[id] = latenciesLocal.Max();
            _latmin[id] = latenciesLocal.Min();
            _latav[id] = latenciesLocal.Average();
        }
        
        private double[] _lat;
        private int[] _tunit;

        private async Task<double> PopulateUnit(IEnumerable<StorageUnit> units, int id, IGrainFactory client) {
            var t = Stopwatch.StartNew();
            await client.GetGrain<IArticleDispatcherWorker>(0).DispatchStorageUnits(units.ToList().AsImmutable());
            _ops += units.Sum(u => u.EntityCount);
            _unit += units.Count();
            _tunit[id] += units.Count();
            return t.ElapsedMilliseconds / units.Count();
        }
        
        private void Init() {
            
            if (string.IsNullOrEmpty(Input)) {
                Input = "/exportcli" + "/out.ddbmsp";
            }
        }
    }
}