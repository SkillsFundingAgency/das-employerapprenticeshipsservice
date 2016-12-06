using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    //TODO add validator and unit tests
    public class GetEmployerAgreementQueryHandler : IAsyncRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public GetEmployerAgreementQueryHandler(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }

        public async Task<GetEmployerAgreementResponse> Handle(GetEmployerAgreementRequest message)
        {
            var caller = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            if (caller == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You are not a member of this Account" } });
            if (caller.RoleId != (int)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You must be an owner of this Account" } });

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(_hashingService.DecodeValue(message.HashedAgreementId));

            if (agreement == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement could not be found" } });
            if (agreement.AccountId != caller.AccountId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement is not linked to this account" } });

            return new GetEmployerAgreementResponse
            {
                EmployerAgreement = agreement
            };
        }
    }
}