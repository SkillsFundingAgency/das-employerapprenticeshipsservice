using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryValidator : IValidator<GetEmployerAgreementRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAgreementQueryValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
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

            var membership = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

            if (membership == null)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}
