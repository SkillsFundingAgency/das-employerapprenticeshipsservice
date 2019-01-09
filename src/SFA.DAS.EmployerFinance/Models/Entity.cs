using System;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.EmployerFinance.Models
{
    public abstract class Entity
    {
        protected void Publish<T>(Func<T> action) where T : class
        {
            UnitOfWorkContext.AddEvent(action);
        }
    }
}