using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.Testing
{
    public class TestableEventPublisher : IEventPublisher
    {
        public IEnumerable<Event> Events => _events;

        private readonly ConcurrentStack<Event> _events = new ConcurrentStack<Event>();

        public Task Publish<T>(Action<T> action) where T : Event, new()
        {
            var message = new T();

            action(message);
            _events.Push(message);

            return Task.CompletedTask;
        }
    }
}