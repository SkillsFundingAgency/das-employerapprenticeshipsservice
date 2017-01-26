using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommandHandler : AsyncRequestHandler<SignEmployerAgreementCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly SignEmployerAgreementCommandValidator _validator;

        public SignEmployerAgreementCommandHandler(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _validator = new SignEmployerAgreementCommandValidator();
        }

        protected override async Task HandleCore(SignEmployerAgreementCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            if (owner == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if ((Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);
            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            if (agreement == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", $"Agreement {message.HashedAgreementId} does not exist" } });
            if (agreement.Status == EmployerAgreementStatus.Signed)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", $"Agreement {message.HashedAgreementId} has already been signed" } });
            if (agreement.ExpiredDate.HasValue)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", $"Agreement {message.HashedAgreementId} has expired" } });

            await _employerAgreementRepository.SignAgreement(agreementId, owner.UserId, $"{owner.FirstName} {owner.LastName}", message.SignedDate);
        }
    }
}