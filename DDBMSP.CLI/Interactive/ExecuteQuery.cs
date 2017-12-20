﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.Common;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.CLI.Interactive
{
    [Verb("query", HelpText = "Create, compile and commit a new named query to the cluster")]
    public class ExecuteQuery
    {
        [Option('n', "name", Required = true, HelpText = "Name of the to-be-executed query")]
        public string Name { get; set; }
        
        [Option('p', "pipe", Required = true, HelpText = "Name of the variable to pipe result into")]
        public string VariableName { get; set; }

        public async Task<int> Run(CSharpRepl repl) {
            var querier = GrainClient.GrainFactory.GetGrain<IGenericQuerier>(0);
            try {
                var t = Stopwatch.StartNew();
                var res = await querier.Query(Name.AsImmutable());
                Console.WriteLine($"{t.ElapsedMilliseconds}ms");

                await repl.AddToState(res.Value, VariableName);
                return 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
    }
}