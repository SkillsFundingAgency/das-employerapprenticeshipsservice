using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IExpiredFundsRepository
    {
        Task Create(long accountId, IEnumerable<ExpiredFund> expiredFunds);
        Task<IEnumerable<ExpiredFund>> Get(long accountId);
    }
}
