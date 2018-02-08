using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;
using Orleans.Runtime;

namespace Grains
{
    public class Corn : Grain, ICorn, IRemindable
    {
        const string ReminderNamePop = "pop";
        private static readonly TimeSpan popMin = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan popRange = TimeSpan.FromMinutes(2);
        private static readonly int popRangeMs = (int)popRange.TotalMilliseconds;

        private bool ShouldLog => this.GetPrimaryKeyLong() % 1000 == 0; // don't want to make too much noise

        public Corn()
        {
            // I removed the logger from here, trying to make thing slimmer
        }

        public async Task BeginPop()
        {
            var pkValue = this.GetPrimaryKeyLong();

            if (ShouldLog)
            {
                Console.WriteLine($"Begin pop: {pkValue}");
            }

            var popTime = popMin + TimeSpan.FromMilliseconds(pkValue % popRangeMs);
            await RegisterOrUpdateReminder(ReminderNamePop, popTime, popTime);
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName == ReminderNamePop)
            {
                await Pop();
            }

            var reminder = await GetReminder(reminderName);
            await UnregisterReminder(reminder);
        }

        private Task Pop()
        {
            if (ShouldLog)
            {
                Console.WriteLine($"POP: {this.GetPrimaryKeyLong()}");
            }

            return Task.CompletedTask;
        }

        public override Task OnActivateAsync()
        {
            // I should try assigning to should log here later maybe to speed things up?

            if (ShouldLog)
            {
                Console.WriteLine($"Activating: {this.GetPrimaryKeyLong()}");
            }

            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            if (ShouldLog)
            {
                Console.WriteLine($"Deactivating: {this.GetPrimaryKeyLong()}");
            }

            return base.OnDeactivateAsync();
        }
    }
}
