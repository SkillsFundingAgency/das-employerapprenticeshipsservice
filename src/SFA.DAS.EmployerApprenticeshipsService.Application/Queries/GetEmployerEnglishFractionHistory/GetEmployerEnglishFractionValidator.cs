using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionValidator : IValidator<GetEmployerEnglishFractionQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerEnglishFractionValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerEnglishFractionQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerEnglishFractionQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.EmpRef))
            {
                validationResult.AddError(nameof(item.EmpRef));
            }
            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId));
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var result = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

            validationResult.IsUnauthorized = result == null;

            return validationResult;
        }
    }
}