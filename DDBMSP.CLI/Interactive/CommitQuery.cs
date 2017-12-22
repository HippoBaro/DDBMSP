using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.CLI.Interactive
{
    [Verb("newquery", HelpText = "Create, compile and commit a new named query to the cluster")]
    public class CommitQuery
    {
        [Option('n', "name", Required = true, HelpText = "Name of the to-be-created query")]
        public string Name { get; set; }
        
        [Option('r', "ressource", Required = true, HelpText = "Ressource to query from")]
        public string TargetRessource { get; set; }
        
        [Option('t', "type", Required = true,
            HelpText = "Expected return type of the query (ex. bool, int, IEnumerable<ArticleState>, etc.")]
        public string Type { get; set; }
        
        [Option('s', "selector", Required = true, HelpText = "Linq selection predicate")]
        public string Selector { get; set; }
        
        [Option('a', "aggregator", Required = true, HelpText = "Linq map-reduce aggregation")]
        public string Aggregator { get; set; }

        public async Task<int> Run() {
            var querier = GrainClient.GrainFactory.GetGrain<IQueryRepository>(0);
            
            var query = new QueryDefinition {
                AggregationLambda = Aggregator,
                SelectorLambda = Selector,
                ReturnTypeName = Type,
                Name = Name,
                TargetRessource = TargetRessource
            };
            
            Console.WriteLine($"Compiling \"{Name}\" definition...");
            try {
                var t = Stopwatch.StartNew();
                await querier.CommitQuery(query.AsImmutable());
                Console.WriteLine($"Succesfully compiled \"{Name}\" definition in {t.ElapsedMilliseconds}ms");
                Console.WriteLine($"\"{Name}\" is now globally available.");
                return 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
    }
}