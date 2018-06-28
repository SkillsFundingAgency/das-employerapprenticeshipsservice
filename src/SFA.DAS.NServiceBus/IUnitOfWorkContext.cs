using System.Collections.Generic;

namespace SFA.DAS.NServiceBus
{
    public interface IUnitOfWorkContext
    {
        T Get<T>();
        void Set<T>(T value);
        IEnumerable<Event> GetEvents();
    }
}