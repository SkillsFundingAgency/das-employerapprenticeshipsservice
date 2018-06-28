using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class EmployerFinancialDbContextExtensions
    {
        public static async Task<TransferAllowance> GetTransferAllowance(this EmployerFinancialDbContext db, long accountId, decimal transferAllowancePercentage)
        {
            var transferAllowance = await db.SqlQueryAsync<TransferAllowance>(
                "[employer_financial].[GetAccountTransferAllowance] @accountId = {0}, @allowancePercentage = {1}",
                accountId,
                transferAllowancePercentage).ConfigureAwait(false);

            return transferAllowance.SingleOrDefault() ?? new TransferAllowance();
        }

        public static async Task<IEnumerable<AccountTransfer>> GetTransfersByTargetAccountId(this EmployerFinancialDbContext db, long accountId, long targetAccountId, string periodEnd)
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

