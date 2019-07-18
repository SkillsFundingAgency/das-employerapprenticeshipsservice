using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    internal class GetAccountQuery : IGetAccountQuery
    {
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;

        public GetAccountQuery(IAccountsReadOnlyRepository accountsReadOnlyRepository)
        {
            _accountsReadOnlyRepository = accountsReadOnlyRepository;
        }

        public async Task<Account> Get(long accountId, CancellationToken cancellationToken = default)
        {
            var document = await _accountsReadOnlyRepository.CreateQuery()
                .FirstOrDefaultAsync( a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
                .ConfigureAwait(false);

            return document?.Account;
        }
    }
}