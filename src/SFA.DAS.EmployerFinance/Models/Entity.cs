using System;
using SFA.DAS.EmployerFinance.MarkerInterfaces;
using SFA.DAS.HashingService;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerFinance.Models
{
    public abstract class Entity
    {
        protected IHashingService _hashingService;
        protected IPublicHashingService _publicHashingService;

        protected void Publish<T>(Action<T> action) where T : new()
        {
            UnitOfWorkContext.AddEvent<object>(() =>
            {
                var message = new T();
                action(message);
                return message;
            });
        }

        public IHashingService HashingService
        {
            set { _hashingService = value; }
        }

        public IPublicHashingService PublicHashingService
        {
            set { _publicHashingService = value; }
        }
    }
}