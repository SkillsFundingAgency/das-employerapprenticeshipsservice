using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Transfers;

namespace SFA.DAS.EmployerAccounts.Data
{
    public static class EmployerFinanceDbContextExtensions
    {
        public static async Task<TransferAllowance> GetTransferAllowance(this EmployerFinanceDbContext db, long accountId, decimal transferAllowancePercentage)
        {
            var transferAllowance = await db.SqlQueryAsync<TransferAllowance>(
                "[employer_financial].[GetAccountTransferAllowance] @accountId = {0}, @allowancePercentage = {1}",
                accountId,
                transferAllowancePercentage).ConfigureAwait(false);

            return transferAllowance.SingleOrDefault() ?? new TransferAllowance();
        }
    }
}

