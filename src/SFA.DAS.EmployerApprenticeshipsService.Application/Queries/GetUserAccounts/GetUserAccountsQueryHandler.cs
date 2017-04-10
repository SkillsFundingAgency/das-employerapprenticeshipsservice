using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryHandler : IAsyncRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>
    {
        private readonly IUserAccountRepository _userAccountsRepository;

        public GetUserAccountsQueryHandler(IUserAccountRepository userAcountRepository)
        {
            _userAccountsRepository = userAcountRepository;
        }

        public async Task<GetUserAccountsQueryResponse> Handle(GetUserAccountsQuery message)
        {
            //TODO add validator.
            var userRef = message.UserRef;

            var accounts = await _userAccountsRepository.GetAccountsByUserRef(userRef);
            return new GetUserAccountsQueryResponse {Accounts = accounts};
        }
    }
}