using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IStatisticsRepository
    {
        Task<Statistics> GetStatistics();
    }
}
