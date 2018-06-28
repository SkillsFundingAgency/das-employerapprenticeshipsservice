using System;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IEventPublisher
    {
        Task Publish<T>(Action<T> action) where T : Event, new();
    }
}