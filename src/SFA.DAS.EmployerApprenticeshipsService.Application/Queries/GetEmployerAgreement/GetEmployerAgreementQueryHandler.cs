using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryHandler : IAsyncRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public GetEmployerAgreementQueryHandler(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<GetEmployerAgreementResponse> Handle(GetEmployerAgreementRequest message)
        {
            var caller = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (caller == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You are not a member of this Account" } });
            if (caller.RoleId != (int)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You must be an owner of this Account" } });

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(message.AgreementId);

            if (agreement == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement could not be found" } });
            if (agreement.AccountId != message.AccountId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement is not linked to this account" } });

            return new GetEmployerAgreementResponse
            {
                EmployerAgreement = agreement
            };
        }
    }
}