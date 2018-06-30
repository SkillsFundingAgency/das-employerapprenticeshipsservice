using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SFA.DAS.NServiceBus
{
    public class UnitOfWorkContext : IUnitOfWorkContext
    {
        private static readonly AsyncLocal<ConcurrentStack<Func<Event>>> Events = new AsyncLocal<ConcurrentStack<Func<Event>>>();

        private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

        public UnitOfWorkContext()
        {
            Events.Value = new ConcurrentStack<Func<Event>>();
        }

        public static void AddEvent<T>(Action<T> action) where T : Event, new()
        {
            Events.Value.Push(() =>
            {
                var message = new T();

                action(message);

                return message;
            });
        }

        public T Get<T>()
        {
            return (T)_data[typeof(T).FullName];
        }

        public IEnumerable<Event> GetEvents()
        {
            return Events.Value.Select(e => e());
        }

        public void Set<T>(T value)
        {
            _data[typeof(T).FullName] = value;
        }
    }
}