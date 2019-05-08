using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Models;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    //todo: internal
    public class GetAccountQuery
    {
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;

        public GetAccountQuery(IAccountsReadOnlyRepository accountsReadOnlyRepository)
        {
            _accountsReadOnlyRepository = accountsReadOnlyRepository;
        }

        public async Task<IAccountDto<IAccountLegalEntityDto<IReservedFundingDto>>> Get(long accountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            //todo: throw if doesn't exist?
            return await _accountsReadOnlyRepository.CreateQuery()
                .FirstOrDefaultAsync( a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}