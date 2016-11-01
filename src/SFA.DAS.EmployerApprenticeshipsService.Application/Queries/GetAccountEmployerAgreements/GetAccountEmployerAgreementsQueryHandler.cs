using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    //TODO tests need adding and validator
    public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IHashingService _hashingService;

        public GetAccountEmployerAgreementsQueryHandler(IMembershipRepository membershipRepository, IAccountRepository accountRepository,IHashingService hashingService)
        {
            _membershipRepository = membershipRepository;
            _accountRepository = accountRepository;
            _hashingService = hashingService;
        }

        public async Task<GetAccountEmployerAgreementsResponse> Handle(GetAccountEmployerAgreementsRequest message)
        {
            var membership = await _membershipRepository.GetCaller(message.HashedId, message.ExternalUserId);

            if (membership == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not a member of this account" } });
            if (membership.RoleId != (short)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not an owner of this account" } });

            var agreements = await _accountRepository.GetEmployerAgreementsLinkedToAccount(membership.AccountId);

            foreach (var agreement in agreements)
            {
                agreement.HashedAgreementId = _hashingService.HashValue(agreement.Id);
            }

            return new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = agreements
            };
        }
    }
}