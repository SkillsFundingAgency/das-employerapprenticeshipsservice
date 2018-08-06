using System;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUnitOfWorkManager
    {
        void Begin();
        void End(Exception ex = null);
    }
}