using System;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IUnitOfWorkContext _unitOfWorkContext;

        public EventPublisher(IUnitOfWorkContext unitOfWorkContext)
        {
            _unitOfWorkContext = unitOfWorkContext;
        }

        public Task Publish<T>(Action<T> action) where T : Event, new()
        {
            _unitOfWorkContext.AddEvent(action);

            return Task.CompletedTask;
        }
    }
}