using System;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public interface IUnitOfWorkManager
    {
        void Begin();
        void End(Exception ex = null);
    }
}