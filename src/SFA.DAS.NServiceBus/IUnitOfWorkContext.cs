using System;
using System.Collections.Generic;

namespace SFA.DAS.NServiceBus
{
    public interface IUnitOfWorkContext
    {
        void AddEvent<T>(Action<T> action) where T : Event, new();
        T Get<T>();
        IEnumerable<Event> GetEvents();
        void Set<T>(T value);
    }
}