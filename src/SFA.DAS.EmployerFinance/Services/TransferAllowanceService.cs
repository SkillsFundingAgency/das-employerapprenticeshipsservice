using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class TransferAllowanceService : ITransferAllowanceService
    {
        private readonly EmployerFinanceDbContext _db;
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public TransferAllowanceService(EmployerFinanceDbContext db, LevyDeclarationProviderConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public Task<decimal> GetTransferAllowance(long accountId)
        {
            return _db.GetTransferAllowance(accountId, _configuration.TransferAllowancePercentage);
        }
    }
}