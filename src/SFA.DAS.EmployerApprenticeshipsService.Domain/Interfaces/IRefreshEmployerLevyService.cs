using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IRefreshEmployerLevyService
    {
        Task QueueRefreshLevyMessage(long accountId, string payeRef);
    }
}
