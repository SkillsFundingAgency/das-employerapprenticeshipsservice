using System;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public class EventPublisher : IEventPublisher
    {
        public Task Publish<T>(Action<T> action) where T : Event, new()
        {
            UnitOfWorkContext.AddEvent(action);

            return Task.CompletedTask;
        }
    }
}