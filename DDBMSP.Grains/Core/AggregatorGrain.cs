using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace DDBMSP.Grains.Core
{
    public class AggregatorGrain<TAggregated> : Grain<List<TAggregated>> where TAggregated : class
    {
        private StreamSubscriptionHandle<TAggregated> _consumerHandle;

        protected Task OnActivateAsync(string streamNamespace)
        {
            var streamProvider = GetStreamProvider("Default");
            var obs = streamProvider.GetStream<TAggregated>(this.GetPrimaryKey(), streamNamespace);
            return Task.WhenAll(obs.SubscribeAsync(OnNextAsync, OnErrorAsync, OnCompletedAsync)
                .ContinueWith(task => _consumerHandle = task.Result), base.OnActivateAsync());
        }

        protected virtual Task Aggregate(TAggregated newValue)
        {
            return Task.CompletedTask;
        }
        
        private Task OnNextAsync(TAggregated item, StreamSequenceToken token = null )
        {
            Aggregate(item);
            return Task.CompletedTask;
        }
        
        private Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }
        
        private Task OnErrorAsync(Exception ex)
        {
            Console.WriteLine($"Error : {ex}");
            return Task.CompletedTask;
        }
    }
}