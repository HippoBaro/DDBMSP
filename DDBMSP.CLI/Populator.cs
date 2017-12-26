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
                reader.ReadRootValueAsArray = true;
                Units = serializer.Deserialize<List<StorageUnit>>(reader);
                BytesPerUnit = (int) (s.Length / Units.Count);
            }
        }
        
        private static double Percentile(List<double> sequence, double excelPercentile) {
            sequence.Sort();
            var N = sequence.Count;
            var n = (N - 1) * excelPercentile + 1;
            if (n == 1d) return sequence[0];
            if (n == N) return sequence[N - 1];

            var k = (int) n;
            var d = n - k;
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }

        private static IEnumerable<List<T>> splitList<T>(List<T> locations, int nSize) {
            for (var i = 0; i < locations.Count; i += nSize) {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        private Task Upload() {
            var sublists = splitList(Units, Units.Count / Environment.ProcessorCount).ToList();
            var tasks = new List<Task>(sublists.Count);
            
            lat999 = new List<double>(sublists.Count);
            lat99 = new List<double>(sublists.Count);
            lat95 = new List<double>(sublists.Count);
            latmax = new List<double>(sublists.Count);
            latmin = new List<double>(sublists.Count);
            latav = new List<double>(sublists.Count);
            
            for (var i = 0; i < sublists.Count; i++) {
                lat999.Add(0);
                lat99.Add(0);
                lat95.Add(0);
                latmax.Add(0);
                latmin.Add(0);
                latav.Add(0);
                tasks.Add(Upload(sublists[i], i));
            }

            var output = new Task(async () => {
                while (true) {
                    await Task.Delay(1000);
                    Console.WriteLine($"Uploading... {_ops} ops/sec, {_unit * BytesPerUnit / 1000000}MB/s — Latency: Min = {latmin.Min()}ms, Max = {latmax.Max()}ms, Average = {latav.Average():F3}ms, 95% = {Percentile(lat95, .95):F3}ms, 99% = {Percentile(lat99, .99):F3}ms, 99.9% = {Percentile(lat999, .999):F3}ms\n");
                    _ops = 0;
                    _unit = 0;
                }
            });
            output.Start();
            return Task.WhenAny(Task.WhenAll(tasks), output).ContinueWith(task => {
                Console.WriteLine($"Done. Latency: Min = {latmin.Min()}ms, Max = {latmax.Max()}ms, Average = {latav.Average():F3}ms, 95% = {Percentile(lat95, .95):F3}ms, 99% = {Percentile(lat99, .99):F3}ms, 99.9% = {Percentile(lat999, .999):F3}ms");
            });
        }
        
        int _ops;
        int _unit;

        private List<double> lat999;
        private List<double> lat99;
        private List<double> lat95;
        private List<double> latmax;
        private List<double> latmin;
        private List<double> latav;

        private Task Upload(List<StorageUnit> unitsSubset, int id) {
            var latenciesLocal = new List<double>(unitsSubset.Count);
            var totalunit = 0;
            double lat = 0;
            
            for (var i = 0; i < unitsSubset.Count / Environment.ProcessorCount; i++) {
                PopulateUnit(unitsSubset.Skip(i * Environment.ProcessorCount).Take(Environment.ProcessorCount), ref _ops, ref lat, ref _unit, ref totalunit);
                latenciesLocal.Add(lat);
                lat999[id] = Percentile(latenciesLocal, .999);
                lat99[id] = Percentile(latenciesLocal, .99);
                lat95[id] = Percentile(latenciesLocal, .95);
                latmax[id] = latenciesLocal.Max();
                latmin[id] = latenciesLocal.Min();
                latav[id] = latenciesLocal.Average();
            }
            if (totalunit < unitsSubset.Count) {
                PopulateUnit(unitsSubset.Skip(totalunit).Take(unitsSubset.Count - totalunit), ref _ops, ref lat, ref _unit, ref totalunit);
            }
            return Task.CompletedTask;
        }

        private void PopulateUnit(IEnumerable<StorageUnit> units, ref int ops, ref double lat, ref int unit, ref int tunit) {
            var t = Stopwatch.StartNew();
            ClusterClient.GetGrain<IArticleDispatcherWorker>(0).DispatchStorageUnits(units.ToList().AsImmutable()).Wait();
            lat = t.ElapsedMilliseconds / units.Sum(u => u.EntityCount);;
            ops += units.Sum(u => u.EntityCount);
            unit += units.Count();
            tunit += units.Count();
        }
        
        private void Init() {
            
            if (string.IsNullOrEmpty(Input)) {
                Input = "/exportcli" + "/out.ddbmsp";
            }
        }
    }
}