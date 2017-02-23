using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryHandler : IAsyncRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetEmployerAgreementRequest> _validator;
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAgreementQueryHandler(IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService, IValidator<GetEmployerAgreementRequest> validator, IMembershipRepository membershipRepository)
        {
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _validator = validator;
            _membershipRepository = membershipRepository;
        }

        public async Task<GetEmployerAgreementResponse> Handle(GetEmployerAgreementRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(_hashingService.DecodeValue(message.HashedAgreementId));

            if (agreement == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement could not be found" } });
            if (agreement.HashedAccountId != message.HashedAccountId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement is not linked to this account" } });

            if (agreement.Status != EmployerAgreementStatus.Signed)
            {
                var user = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);
                agreement.SignedByName = $"{user.FirstName} {user.LastName}";
            }

            return new GetEmployerAgreementResponse
            {
                EmployerAgreement = agreement
            };
        }
    }
}