using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class EmployerFinancialDbContextExtensions
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

