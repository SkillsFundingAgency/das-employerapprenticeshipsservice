using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommandHandler : AsyncRequestHandler<SignEmployerAgreementCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly SignEmployerAgreementCommandValidator _validator;

        public SignEmployerAgreementCommandHandler(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _validator = new SignEmployerAgreementCommandValidator();
        }

        protected override async Task HandleCore(SignEmployerAgreementCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.HashedId, message.ExternalUserId);

            if (owner == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if ((Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(message.AgreementId);

            if (agreement == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", $"Agreement {message.AgreementId} does not exist" } });
            if (agreement.Status == EmployerAgreementStatus.Signed)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", $"Agreement {message.AgreementId} has already been signed" } });
            if (agreement.ExpiredDate.HasValue)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", $"Agreement {message.AgreementId} has expired" } });

            await _employerAgreementRepository.SignAgreement(message.AgreementId, owner.UserId, $"{owner.FirstName} {owner.LastName}", message.SignedDate);
        }
    }
}