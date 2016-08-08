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
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountPayeSchemesQueryHandler(IAccountRepository accountRepository, IMembershipRepository membershipRepository)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesRequest message)
        {
            var membership = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (membership == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not a member of this account" } });
            if (membership.RoleId != (short)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not an owner of this account" } });

            var payeSchemes = await _accountRepository.GetPayeSchemes(message.AccountId);

            return new GetAccountPayeSchemesResponse
            {
                PayeSchemes = payeSchemes
            };
        }
    }
}