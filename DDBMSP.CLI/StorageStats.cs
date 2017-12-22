using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.Entities;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
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
    [Verb("stats", HelpText = "Get statistics on storage")]
    internal class StorageStats
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
            var hash = GrainClient.GrainFactory.GetGrain<IDistributedHashTable<Guid, TRessource>>(0);

            var stats = await hash.GetBucketUsage();
            
            Console.WriteLine($"\tTotal: {stats.Sum()}");
            Console.WriteLine($"\tAverage: {stats.Average()}");
            Console.WriteLine($"\tMin: {stats.Min()}");
            Console.WriteLine($"\tMax: {stats.Max()}");
            Console.WriteLine($"\tDelta: {stats.Max() - stats.Min()}");
        }
        
        private static void Connect() {
            var config = ClientConfiguration.LocalhostSilo();
            config.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
            
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
        
        private void Init() {
            
            Connect();
            
            if (string.IsNullOrEmpty(Input)) {
                Input = Environment.CurrentDirectory + "/out.ddbmsp";
            }
        }
    }
}