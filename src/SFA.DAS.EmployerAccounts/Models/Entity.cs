using System;
using SFA.DAS.NServiceBus;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerAccounts.Models
{
    public abstract class Entity
    {
        protected void Publish<T>(Action<T> action) where T : Event, new()
        {
            UnitOfWorkContext.AddEvent<object>(() =>
            {
                var message = new T();
                action(message);
                return message;
            });
        }
    }
}