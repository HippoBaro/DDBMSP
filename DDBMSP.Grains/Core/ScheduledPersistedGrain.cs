using System;
using System.Threading.Tasks;
using DDBMSP.Common;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class ScheduledPersistedGrain<T> : SingleWriterMultipleReadersGrain<T> where T : new()
    {
        private bool Dirty { get; set; }
        private DateTime LastCommit { get; set; } = DateTime.MinValue;
        private bool IsSynchingSchedulded { get; set; }

        protected void CommitChanges() {
            Console.WriteLine($"CommitChanges from {this.GetType().Name} #{this.GetPrimaryKeyLong()}");
            LastCommit = DateTime.Now;
            Dirty = true;
        }
        
        public override Task OnActivateAsync() {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(25000, 35000));
            RegisterTimer(Flush, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }
        
        private Task Flush(object _) {
            var elapsed = DateTime.Now.Subtract(LastCommit).TotalSeconds;
            if (!Dirty || !(elapsed > 10) || IsSynchingSchedulded) return Task.CompletedTask;
            
            Console.WriteLine($"Synching #{this.GetPrimaryKeyLong()}. Delta {elapsed}s. Dirty: {Dirty}. Is IsSynchingSchedulded: {IsSynchingSchedulded}.");
            LastCommit = DateTime.UtcNow;
            Dirty = false;
            IsSynchingSchedulded = true;
            return SerialExecutor.AddNext(async () => {
                await WriteStateAsync();
                IsSynchingSchedulded = false;
            });
        }
    }
}