using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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

        public async Task<AccountDto> Get(long accountId, CancellationToken cancellationToken = default)
        {
            // it seems using get by id when using partitioning, need to supply a partition key
            // need to bake it into repo
            var options = new RequestOptions {PartitionKey = new PartitionKey(accountId)};
            var account = await _accountsReadOnlyRepository.GetById(accountId, options, cancellationToken: cancellationToken);
            return account?.Deleted != null ? account : null;
//            return await _accountsReadOnlyRepository.CreateQuery()
//                .FirstOrDefaultAsync( a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
//                .ConfigureAwait(false);
        }
    }
}