using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class EmployerFinancialDbContextExtensions
    {
        public static async Task<decimal> GetTransferAllowance(this EmployerFinancialDbContext db, long accountId, decimal transferAllowancePercentage)
        {
            var transferAllowance = await db.SqlQueryAsync<decimal?>(
                "[employer_financial].[GetAccountTransferAllowance] @accountId = {0}, @allowancePercentage = {1}",
                accountId,
                transferAllowancePercentage);

            return transferAllowance.SingleOrDefault() ?? 0;
        }

        public static async Task<IEnumerable<AccountTransfer>> GetTransfersByTargetAccountId(
            this EmployerFinancialDbContext db, long accountId, long targetAccountId, string periodEnd)
        {
            var transfers = await db.SqlQueryAsync<AccountTransfer>(
                "[employer_financial].[GetTransferTransactionDetails] @accountId = {0}, @targetAccountId = {1}, @periodEnd = {2}",
                accountId,
                targetAccountId,
                periodEnd);

            return transfers;
        }

        public static async Task<IEnumerable<AccountTransfer>> GetTransfersSentInCurrentFinancialYear(
            this EmployerFinancialDbContext db, long accountId)
        {
            var transfers = await db.SqlQueryAsync<AccountTransfer>(
                "[employer_financial].[GetSenderAccountTransactionsInCurrentFinancialYear] @senderAccountId = {0}",
                accountId);

            return transfers;
        }
    }
}

