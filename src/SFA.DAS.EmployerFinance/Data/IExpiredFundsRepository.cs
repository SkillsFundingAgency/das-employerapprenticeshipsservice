using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IExpiredFundsRepository
    {
        Task Create(long accountId, IEnumerable<ExpiredFund> expiredFunds, DateTime now);
        Task<IEnumerable<ExpiredFund>> Get(long accountId);
    }
}
