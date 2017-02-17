using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommandValidator : IValidator<SignEmployerAgreementCommand>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public SignEmployerAgreementCommandValidator(IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }

        public ValidationResult Validate(SignEmployerAgreementCommand item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(SignEmployerAgreementCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAgreementId))
                validationResult.AddError(nameof(item.HashedAgreementId));

            if (string.IsNullOrEmpty(item.HashedAccountId))
                validationResult.AddError(nameof(item.HashedAccountId));

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError(nameof(item.ExternalUserId));

            if (item.SignedDate == default(DateTime))
                validationResult.AddError(nameof(item.SignedDate));

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var agreementId = _hashingService.DecodeValue(item.HashedAgreementId);
            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            if (agreement == null)
            {
                validationResult.AddError(nameof(agreement), "Agreement does not exist");
                return validationResult;
            }

            if (agreement.Status == EmployerAgreementStatus.Signed ||
                agreement.Status == EmployerAgreementStatus.Expired ||
                agreement.Status == EmployerAgreementStatus.Superceded)
            {
                validationResult.AddError(nameof(agreement.Status),$"Agreement status is {agreement.Status}");
                return validationResult;
            }
            
            return validationResult;
        }
    }
}