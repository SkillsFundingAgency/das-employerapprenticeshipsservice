using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Transfers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class EmployerFinancialDbContextExtensions
    {
        public static async Task<decimal> GetTransferAllowance(this EmployerFinanceDbContext db, long accountId, decimal transferAllowancePercentage)
        {
            var transferAllowance = await db.SqlQueryAsync<decimal?>(
                "[employer_financial].[GetAccountTransferAllowance] @accountId = {0}, @allowancePercentage = {1}",
                accountId,
                transferAllowancePercentage).ConfigureAwait(false);

            return transferAllowance.SingleOrDefault() ?? 0;
        }

        public static async Task<IEnumerable<AccountTransfer>> GetTransfersByTargetAccountId(this EmployerFinanceDbContext db, long accountId, long targetAccountId, string periodEnd)
        {
            var transfers = await db.SqlQueryAsync<AccountTransfer>(
                "[employer_financial].[GetTransferTransactionDetails] @accountId = {0}, @targetAccountId = {1}, @periodEnd = {2}",
                accountId,
                targetAccountId,
                periodEnd).ConfigureAwait(false);

            return transfers;
        }
    }
}

