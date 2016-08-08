using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHandler : IAsyncRequestHandler<GetEmployerAccountQuery, GetEmployerAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository   ;

        public GetEmployerAccountHandler(IEmployerAccountRepository employerAccountRepository)
        {
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetEmployerAccountResponse> Handle(GetEmployerAccountQuery message)
        {
            var employerAccount = await _employerAccountRepository.GetAccountById(message.Id);
            return new GetEmployerAccountResponse {Account = employerAccount};
        }
    }
}
