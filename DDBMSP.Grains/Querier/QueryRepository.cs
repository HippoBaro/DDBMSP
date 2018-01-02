using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Common.QueryEngine;
using DDBMSP.Entities.Query;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans.Concurrency;
using Orleans.Providers;

namespace DDBMSP.Grains.Querier
{
    [Reentrant]
    [StorageProvider(ProviderName = "RedisStore")]
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
             
            newquery -n test -t "KeyValuePair<Guid, ArticleState>" -s "Articles.Where(pair => pair.Value.Title != null)" -a "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value).ToList()"
            newquery -n test1k -t "KeyValuePair<Guid, ArticleState>" -s "Articles.Where(pair => pair.Value.Title != null)" -a "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value).Take(1000).ToList()"
            
            newquery -n test -r User -t "int" -s "Elements.Count()" -a "Selected.Sum()"
            newquery -n test1k -r Article -t "IEnumerable<ArticleState>" -s "Elements.Where(article => article.Title != null)" -a "Selected.SelectMany(d => d).Take(1000).ToList()"
            
            newquery -n testActivities -r Article -t "IEnumerable<UserActivityState>" -s "Elements.Select(list => list.Where(state => state.User.Name.Contains("Iachin")))" -a "Selected.SelectMany(d => d).ToList()"
            
            query commit -n ActivitiesCount -r Activity -t int -s "Elements.Sum(e => e.Count())" -a "Selected.Sum()"
            query commit -n CommentCount -r Activity -t int -s "Elements.Sum(e => e.Count(a => a.Type == UserActivityType.Commented))" -a "Selected.Sum()"
            
            */
            
            switch (queryDefinition.Value.TargetRessource) {
                case "Article":
                    queryDefinition.Value.TargetRessource = "ArticleState";
                    break;
                case "User":
                    queryDefinition.Value.TargetRessource = "UserState";
                    break;
                case "Activity":
                    queryDefinition.Value.TargetRessource = "List<UserActivityState>";
                    break;
                default:
                    throw new Exception("Unknown ressource type");
            }

            return SerialExecutor.AddNext(() => {
                QueryEngine.CompileAndRegister(queryDefinition.Value);
                State.Add(queryDefinition.Value.Name, queryDefinition.Value);
                return WriteStateAsync();
            });
        }
    }
}