using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferAllowanceSnapshotRepository : ITransferAllowanceSnapshotRepository
    {
        private readonly EmployerFinancialDbContext _db;

        public TransferAllowanceSnapshotRepository(EmployerFinancialDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task UpsertAsync(long accountId, int endYear, decimal transferAllowance)
        {
            try
            {
                var transfer = await _db.AccountTransferSnapshots.FirstOrDefaultAsync(ts => ts.AccountId == accountId && ts.Year == endYear);

                if (transfer == null)
                {
                    transfer = new AccountTransferAllowanceSnapshot
                    {
                        AccountId = accountId,
                        Year = endYear
                    };

                    _db.AccountTransferSnapshots.Add(transfer);
                }

                transfer.SnapshotTime = DateTime.UtcNow;
                transfer.TransferAllowance = transferAllowance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}