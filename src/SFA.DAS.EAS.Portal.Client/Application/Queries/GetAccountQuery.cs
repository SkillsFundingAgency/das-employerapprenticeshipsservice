using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    internal class GetAccountQuery
    {
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;

        public GetAccountQuery(IAccountsReadOnlyRepository accountsReadOnlyRepository)
        {
            _accountsReadOnlyRepository = accountsReadOnlyRepository;
        }

        public async Task<AccountDto> Get(long accountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            //todo: throw if doesn't exist?
            return await _accountsReadOnlyRepository.CreateQuery()
                .FirstOrDefaultAsync( a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}