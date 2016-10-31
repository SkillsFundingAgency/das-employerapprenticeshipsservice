using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccounts
{
    public class GetAccountsQueryHandler : IAsyncRequestHandler<GetEmployerAccountsQuery, GetEmployerAccountsResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetAccountsQueryHandler(IEmployerAccountRepository employerAccountRepository)
        {
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetEmployerAccountsResponse> Handle(GetEmployerAccountsQuery message)
        {

            var accounts = await _employerAccountRepository.GetAccounts(message.ToDate, message.PageNumber, message.PageSize);
            return new GetEmployerAccountsResponse() {AccountsCount = accounts.AccountsCount, Accounts = accounts.AccountList};
        }
    }
}
