using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts
{
    public class GetAllEmplyoerAccountsQueryHandler : IAsyncRequestHandler<GetAllEmployerAccountsRequest, GetAllEmployerAccountsResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetAllEmplyoerAccountsQueryHandler(IEmployerAccountRepository employerAccountRepository)
        {
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetAllEmployerAccountsResponse> Handle(GetAllEmployerAccountsRequest message)
        {
            var result = await _employerAccountRepository.GetAllAccounts();

            return new GetAllEmployerAccountsResponse {Accounts = result };
        }
    }
}