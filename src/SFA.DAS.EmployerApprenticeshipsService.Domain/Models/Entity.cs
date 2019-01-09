using System;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.EAS.Domain.Models
{
    public abstract class Entity
    {
        protected void Publish<T>(Func<T> message) where T : class
        {
            UnitOfWorkContext.AddEvent(message);
        }
    }
}