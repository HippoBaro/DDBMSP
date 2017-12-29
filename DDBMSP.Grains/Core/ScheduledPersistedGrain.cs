using System;
using System.Threading.Tasks;
using DDBMSP.Common;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class ScheduledPersistedGrain<T> : SingleWriterMultipleReadersGrain<T> where T : new()
    {
        private bool HasChanged { get; set; }
        private DateTime LastSynched { get; set; } = DateTime.UtcNow;
        private DateTime LastCommit { get; set; } = DateTime.MinValue;

        protected void CommitChanges() {
            HasChanged = true;
            LastCommit = DateTime.UtcNow;
        }
        
        public override async Task OnActivateAsync() {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(25000, 35000));
            RegisterTimer(Flush, this, targetTicks, targetTicks);
            await base.OnActivateAsync();
        }
        
        private Task Flush(object _) {
            if (!HasChanged) return Task.CompletedTask;
            if ((LastCommit - LastSynched).TotalSeconds > 10) {
                Console.WriteLine($"Synching #{this.GetPrimaryKeyLong()}. Last was {LastCommit:T} - {LastSynched:T}. Delta {(LastCommit - LastSynched).TotalSeconds}");
                LastSynched = LastCommit = DateTime.UtcNow;
                return SerialExecutor.AddNext(async () => {
                    await WriteStateAsync();
                    HasChanged = false;
                });
            }
            return Task.CompletedTask;
        }
    }
}