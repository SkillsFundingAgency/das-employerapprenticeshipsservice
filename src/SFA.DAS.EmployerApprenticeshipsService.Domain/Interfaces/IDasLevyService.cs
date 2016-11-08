using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<List<TransactionLine>> GetTransactionsByAccountId(long accountId);

        Task<List<AccountBalance>> GetAllAccountBalances();
        Task<List<TransactionLineDetail>>  GetTransactionDetailById(long id);
    }
}