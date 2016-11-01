using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IDasLevyRepository _repository;
        
        public DasLevyService(IDasLevyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TransactionLine>> GetTransactionsByAccountId(long accountId)
        {
            var getEmployerAccountTransactionsResponse = await _repository.GetTransactions(accountId);
            return getEmployerAccountTransactionsResponse;
        }
    }
}