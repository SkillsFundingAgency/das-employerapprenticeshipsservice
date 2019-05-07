using SFA.DAS.EAS.Portal.Client.Data;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    internal class GetAccountQuery
    {
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;

        public GetAccountQuery(IAccountsReadOnlyRepository accountsReadOnlyRepository)
        {
            _accountsReadOnlyRepository = accountsReadOnlyRepository;
        }
    }
}