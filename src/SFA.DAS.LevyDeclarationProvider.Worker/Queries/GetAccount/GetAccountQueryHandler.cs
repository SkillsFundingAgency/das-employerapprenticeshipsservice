using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Queries.GetAccount
{
    public class GetAccountQueryHandler : IAsyncRequestHandler<GetAccountRequest, GetAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetAccountQueryHandler(IEmployerAccountRepository employerAccountRepository)
        {
            if (employerAccountRepository == null)
                throw new ArgumentNullException(nameof(employerAccountRepository));
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetAccountResponse> Handle(GetAccountRequest message)
        {
            var employerAccount = await _employerAccountRepository.GetAccountById(message.AccountId);

            return new GetAccountResponse
            {
                Account = employerAccount
            };
        }
    }
}