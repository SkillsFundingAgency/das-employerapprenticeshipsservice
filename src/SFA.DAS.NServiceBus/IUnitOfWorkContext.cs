using System.Collections.Generic;

namespace SFA.DAS.NServiceBus
{
    public interface IUnitOfWorkContext
    {
        T Get<T>();
        IEnumerable<Event> GetEvents();
        void Set<T>(T value);
    }
}