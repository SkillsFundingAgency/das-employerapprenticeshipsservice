﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts
{
    public class GetPagedAccountsQueryHandler : IAsyncRequestHandler<GetPagedEmployerAccountsQuery, GetPagedEmployerAccountsResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetPagedAccountsQueryHandler(IEmployerAccountRepository employerAccountRepository)
        {
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetPagedEmployerAccountsResponse> Handle(GetPagedEmployerAccountsQuery message)
        {
            var accounts = await _employerAccountRepository.GetAccounts(message.ToDate, message.PageNumber, message.PageSize);
            return new GetPagedEmployerAccountsResponse() { AccountsCount = accounts.AccountsCount, Accounts = accounts.AccountList };
        }
    }
}
