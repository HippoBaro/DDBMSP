using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Common.QueryEngine;
using DDBMSP.Entities.Query;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Querier
{
    [Reentrant]
    public class QueryRepository : SingleWriterMultipleReadersGrain<Dictionary<string, QueryDefinition>>,
        IQueryRepository
    {
        public override Task OnActivateAsync() {
            if (State == null) State = new Dictionary<string, QueryDefinition>();
            return base.OnActivateAsync();
        }

        public Task<Immutable<QueryDefinition>> GetQueryDefinition(Immutable<string> name) =>
            Task.FromResult(State[name.Value].AsImmutable());

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
            
            newquery -n test -t "IEnumerable<KeyValuePair<Guid, ArticleState>>" -s "Articles.Where(pair => pair.Value.Title != null)" -a "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value).ToList()"
            newquery -n test1k -t "IEnumerable<KeyValuePair<Guid, ArticleState>>" -s "Articles.Where(pair => pair.Value.Title != null)" -a "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value).Take(1000).ToList()"
            */

            return SerialExecutor.AddNext(() => {
                QueryEngine.CompileAndRegister(queryDefinition.Value);
                State.Add(queryDefinition.Value.Name, queryDefinition.Value);
                return Task.CompletedTask;
            });
        }
    }
}