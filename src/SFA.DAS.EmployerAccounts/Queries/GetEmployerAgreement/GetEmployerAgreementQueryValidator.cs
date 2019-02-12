using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryValidator : IValidator<GetEmployerAgreementRequest>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public GetEmployerAgreementQueryValidator(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }

        public ValidationResult Validate(GetEmployerAgreementRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAgreementRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.AgreementId))
            {
                validationResult.AddError(nameof(item.AgreementId));
            }

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId));
            }

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var caller = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
            
            if (caller == null)
            {
                validationResult.IsUnauthorized = true;
                return validationResult;
            }

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(_hashingService.DecodeValue(item.AgreementId));

            if (agreement == null)
            {
                return validationResult;
            }

            if (agreement.HashedAccountId != item.HashedAccountId || (agreement.Status != EmployerAgreementStatus.Signed && caller.Role != Role.Owner))
            {
                validationResult.IsUnauthorized = true;
            }    
            
            return validationResult;
        }
    }
}
