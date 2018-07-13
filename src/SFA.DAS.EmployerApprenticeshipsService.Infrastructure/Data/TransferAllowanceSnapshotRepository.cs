using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferAllowanceSnapshotRepository : ITransferAllowanceSnapshotRepository
    {
        private readonly EmployerFinancialDbContext _db;
        private readonly ILog _logger;

        public TransferAllowanceSnapshotRepository(EmployerFinancialDbContext dbContext, ILog logger)
        {
            _db = dbContext;
            _logger = logger;
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
                _logger.Error(e, $"Filed to set transfer allowance: accountId:{accountId} endYear:{endYear} transferAllowance:{transferAllowance}");
                throw;
            }
        }
    }
}