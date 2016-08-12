using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IAccountRepository _accountRepository;

        public GetAccountEmployerAgreementsQueryHandler(IMembershipRepository membershipRepository, IAccountRepository accountRepository)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _membershipRepository = membershipRepository;
            _accountRepository = accountRepository;
        }

        public async Task<GetAccountEmployerAgreementsResponse> Handle(GetAccountEmployerAgreementsRequest message)
        {
            var membership = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (membership == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not a member of this account" } });
            if (membership.RoleId != (short)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not an owner of this account" } });

            var agreements = await _accountRepository.GetEmployerAgreementsLinkedToAccount(message.AccountId);

            return new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = agreements
            };
        }
    }
}