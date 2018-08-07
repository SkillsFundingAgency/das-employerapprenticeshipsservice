using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Worker.IdProcessor;
using SFA.DAS.EAS.Account.Worker.Jobs.GenerateAgreements;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Account.Worker.Jobs.SetAccountLegalEntityPublicHashIds
{
    public class SetAccountLegalEntityPublicHashIdsIdProvider : IIdProvider
    {
        private readonly IAccountRepository _accountRepository;

        public SetAccountLegalEntityPublicHashIdsIdProvider(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<long>> GetIdsAsync(long startAfterId, int count, ProcessingContext processingContext)
        {
            return await _accountRepository.GetAccountLegalEntitiesWithoutPublicHashId(
                                            startAfterId + 1, 
                                            count);
        }
    }
}
