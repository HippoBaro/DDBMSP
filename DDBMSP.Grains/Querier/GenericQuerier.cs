using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Querier
{
    [StatelessWorker]
    [Reentrant]
    public class GenericQuerier : Grain, IGenericQuerier
    {
        public Task CommitQuery(Immutable<QueryDefinition> queryDefinition) {
            /*
            var query = new QueryDefinition {
                AggregationLambda = "Selected.Sum()",
                SelectorLambda = "Articles.Count(pair => !string.IsNullOrEmpty(pair.Value.Title))",
                ReturnTypeName = "int"
            };
            
            var query2 = new QueryDefinition {
                AggregationLambda = "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value)",
                SelectorLambda = "Articles.Where(pair => pair.Value.Author.Name.Contains(\"Mira\"))",
                ReturnTypeName = "IEnumerable<KeyValuePair<Guid, ArticleState>>"
            };
            
            newquery -n test -t "IEnumerable<KeyValuePair<Guid, ArticleState>>" -s "Articles.Where(pair => pair.Value.Title != null)" -a "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value)"
            
            */
            
            QueryEngine.CompileAndRegister(queryDefinition.Value);
            return Task.CompletedTask;
        }

        public async Task<Immutable<dynamic>> Query(Immutable<string> queryName) {
            return await GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0)
                .Query(queryName);;
        }
    }
}