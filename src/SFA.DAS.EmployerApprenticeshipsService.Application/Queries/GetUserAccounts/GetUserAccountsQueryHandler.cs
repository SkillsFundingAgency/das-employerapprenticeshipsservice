using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

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
            var userId = message.UserId;

            var accounts = await _userAccountsRepository.GetAccountsByUserId(userId);
            return new GetUserAccountsQueryResponse {Accounts = accounts};
        }
    }
}