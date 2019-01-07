using System;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.EmployerFinance.Models
{
    public abstract class Entity
    {
        protected void Publish<T>(Action<T> action) where T : class, new()
        {
            UnitOfWorkContext.AddEvent(action);
        }
    }
}