using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Transfers;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class TransferAllowanceService : ITransferAllowanceService
    {
        private readonly EmployerFinanceDbContext _db;
        private readonly EmployerFinanceConfiguration _configuration;

        public TransferAllowanceService(EmployerFinanceDbContext db, EmployerFinanceConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public Task<TransferAllowance> GetTransferAllowance(long accountId)
        {
            return _db.GetTransferAllowance(accountId, _configuration.TransferAllowancePercentage);
        }
    }
}