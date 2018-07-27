using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Worker.IdProcessor;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Account.Worker.Jobs.SetAccountLegalEntityPublicHashIds
{
    public class SetAccountLegalEntityPublicHashIdsProcessor : IProcessor
    {
        private readonly IAccountRepository _accountRepository;

        public SetAccountLegalEntityPublicHashIdsProcessor(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> DoAsync(long accountLegalEntityId, ProcessingContext processorContext)
        {
            await _accountRepository.UpdateAccountLegalEntityPublicHashedId(accountLegalEntityId);

            return true;
        }

        public Task<bool> InspectFailedAsync(long id, Exception exception, ProcessingContext processorContext)
        {
            return Task.FromResult(false);
        }
    }
}
