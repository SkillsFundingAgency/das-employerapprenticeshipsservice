using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework.Mvc
{
    public static class IEndpointInstanceTaskExtensions
    {
        public static async Task<IEndpointInstance> SetupOutboxSchedule(this Task<IEndpointInstance> start)
        {
            var endpoint = await start;

            await endpoint.ScheduleEvery(TimeSpan.FromMinutes(10), "ProcessOutboxMessages", p => p.SendLocal(new ProcessOutboxMessagesCommand())).ConfigureAwait(false);

            return endpoint;
        }
    }
}