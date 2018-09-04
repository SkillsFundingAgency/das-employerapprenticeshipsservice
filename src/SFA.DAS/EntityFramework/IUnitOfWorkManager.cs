using System;

namespace SFA.DAS.EntityFramework
{
    public interface IUnitOfWorkManager
    {
        void Begin();
        void End(Exception ex = null);
    }
}