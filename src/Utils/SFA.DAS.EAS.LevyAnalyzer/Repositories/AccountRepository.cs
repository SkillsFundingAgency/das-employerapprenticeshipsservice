using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Models;

namespace SFA.DAS.EAS.LevyAnalyzer.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AccountRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Account> GetAccountAsync(long accountId)
        {
            using (var financeDb = _dbContextFactory.GetFinanceDbContext())
            {
                var txnsTask = await financeDb.GetTransactionsAsync(accountId);

                var levyTask = await financeDb.GetLevyDeclarationsAsync(accountId);

                return new Account(txnsTask, levyTask);
            }
        }
    }
}
