using System.Linq;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.DasRecruitService.Services;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Types;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    internal class GetAccountQuery
    {
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;
        private readonly IDasRecruitService _dasRecruitService;

        public GetAccountQuery(IAccountsReadOnlyRepository accountsReadOnlyRepository,
            IDasRecruitService dasRecruitService)
        {
            _accountsReadOnlyRepository = accountsReadOnlyRepository;
            _dasRecruitService = dasRecruitService;
        }

        public async Task<Account> Get(long accountId, bool getRecruit, CancellationToken cancellationToken = default)
        {
            var document = await _accountsReadOnlyRepository.CreateQuery()
                .FirstOrDefaultAsync( a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
                .ConfigureAwait(false);
            if (getRecruit)
            {
                var vacanciesSummary = _dasRecruitService.GetVacanciesSummary(document.AccountId);
                
            }

            return document?.Account;
        }
    }
}