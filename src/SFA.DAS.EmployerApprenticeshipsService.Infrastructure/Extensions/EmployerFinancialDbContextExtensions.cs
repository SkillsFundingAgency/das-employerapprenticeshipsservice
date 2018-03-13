using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Infrastructure.Data;

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
    }
}