using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHandler : IAsyncRequestHandler<GetEmployerAccountQuery, GetEmployerAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository   ;
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAccountHandler(IEmployerAccountRepository employerAccountRepository, IMembershipRepository membershipRepository)
        {
            if (employerAccountRepository == null)
                throw new ArgumentNullException(nameof(employerAccountRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _employerAccountRepository = employerAccountRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<GetEmployerAccountResponse> Handle(GetEmployerAccountQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (membership == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not a member of this account" } });

            var employerAccount = await _employerAccountRepository.GetAccountById(message.AccountId);

            return new GetEmployerAccountResponse
            {
                Account = employerAccount
            };
        }
    }
}
