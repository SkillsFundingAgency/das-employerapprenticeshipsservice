using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Repositories
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

                return new Account(accountId, txnsTask, levyTask);
            }
        }
    }
}
