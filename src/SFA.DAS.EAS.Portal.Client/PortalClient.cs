using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Encoding;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client
{
    public class PortalClient : IPortalClient
    {
        private readonly IEncodingService _encodingService;
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;
        private readonly IDasRecruitService _dasRecruitService;
        
        public PortalClient(IContainer container, IEncodingService encodingService)
        {
            _encodingService = encodingService;
            _accountsReadOnlyRepository = container.GetInstance<IAccountsReadOnlyRepository>();
            _dasRecruitService = container.GetInstance<IDasRecruitService>();
        }
        
        public async Task<Account> GetAccount(string hashedAccountId, int maxNumberOfVacancies,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(hashedAccountId))
                throw new ArgumentException($"Missing {nameof(hashedAccountId)}");

            if (maxNumberOfVacancies < 0)
                throw new ArgumentOutOfRangeException(nameof(maxNumberOfVacancies));

            var vacanciesTask = maxNumberOfVacancies > 0 ?
                _dasRecruitService.GetVacancies(hashedAccountId, maxNumberOfVacancies, cancellationToken) : null;

            // might have been better to key doc on the public hashed account id
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
            var account = await _accountsReadOnlyRepository.Get(accountId, cancellationToken);

            // at a later date, we might want to create an empty account doc and add the vacancy details to it, but for now, let's keep it simple
            if (vacanciesTask != null && account != null)
            {
                var vacancies = await vacanciesTask;
                if (vacancies != null)
                {
                    account.VacanciesRetrieved = true;
                    account.Vacancies = (await vacanciesTask).ToList();
                }
            }

            return account;
        }
    }
}