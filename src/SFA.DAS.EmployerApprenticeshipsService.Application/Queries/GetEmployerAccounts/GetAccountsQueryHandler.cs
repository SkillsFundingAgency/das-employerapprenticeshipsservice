using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccounts
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

            var accounts = await _employerAccountRepository.GetAccounts(message.FromDate, message.PageNumber, message.PageSize);
            return new GetEmployerAccountsResponse() {AccountsCount = accounts.AccountsCount, Accounts = accounts.AccountList};
        }
    }
}
