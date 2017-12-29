using System;
using System.Threading.Tasks;
using DDBMSP.Common;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class ScheduledPersistedGrain<T> : SingleWriterMultipleReadersGrain<T> where T : new()
    {
        private DateTime LastCommit { get; set; } = DateTime.MinValue;

        protected void CommitChanges() {
            LastCommit = DateTime.Now;
        }
        
        public override Task OnActivateAsync() {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(25000, 35000));
            RegisterTimer(Flush, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }
        
        private Task Flush(object _) {
            var elapsed = DateTime.Now.Subtract(LastCommit).TotalSeconds;
            if (!(elapsed > 10)) return Task.CompletedTask;
            
            Console.WriteLine($"Synching #{this.GetPrimaryKeyLong()}. Delta {elapsed}s");
            LastCommit = DateTime.UtcNow;
            return SerialExecutor.AddNext(WriteStateAsync);
        }
    }
}