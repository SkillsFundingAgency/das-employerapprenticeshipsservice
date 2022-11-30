using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts
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
            var result = await _employerAccountRepository.GetAll();

            return new GetAllEmployerAccountsResponse {Accounts = result };
        }
    }
}