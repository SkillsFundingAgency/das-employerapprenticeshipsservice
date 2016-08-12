using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesRequest, GetAccountPayeSchemesResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountPayeSchemesQueryHandler(IAccountRepository accountRepository)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesRequest message)
        {
            var payeSchemes = await _accountRepository.GetPayeSchemes(message.AccountId);

            return new GetAccountPayeSchemesResponse
            {
                PayeSchemes = payeSchemes
            };
        }
    }
}