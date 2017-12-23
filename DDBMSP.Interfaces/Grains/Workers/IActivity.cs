using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.UserActivity;
using DDBMSP.Interfaces.Converters;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Workers
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IUserActivityWorker : IGrainWithIntegerKey
    {
        Task SetActivitiesForArticle(Immutable<Guid> guid, Immutable<List<UserActivityState>> activities);
        Task SetActivitiesForArticles(Immutable<Dictionary<Guid, List<UserActivityState>>> activities);
        Task AddActivitiesToArticle(Immutable<Guid> guid, Immutable<UserActivityState> activity);
    }
}