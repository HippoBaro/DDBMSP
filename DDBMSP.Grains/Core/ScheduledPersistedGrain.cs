using System;
using System.Threading.Tasks;
using DDBMSP.Common;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class ScheduledPersistedGrain<T> : Grain<T> where T : new()
    {
        private bool HasChanged { get; set; }

        protected void CommitChanges() {
            HasChanged = true;
        }
        
        public override async Task OnActivateAsync() {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(1000, 5000));
            RegisterTimer(Flush, this, targetTicks, targetTicks);
            await base.OnActivateAsync();
        }
        
        private Task Flush(object _) {
            if (HasChanged)
                return WriteStateAsync();
            HasChanged = false;
            return Task.CompletedTask;
        }
    }
}