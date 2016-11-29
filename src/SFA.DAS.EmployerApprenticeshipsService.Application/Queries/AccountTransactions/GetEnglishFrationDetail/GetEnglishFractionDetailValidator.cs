using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailValidator : IValidator<GetEnglishFractionDetailByEmpRefQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEnglishFractionDetailValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEnglishFractionDetailByEmpRefQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEnglishFractionDetailByEmpRefQuery item)
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
            if (string.IsNullOrEmpty(item.AccountId))
            {
                validationResult.AddError(nameof(item.AccountId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var result = await _membershipRepository.GetCaller(item.AccountId, item.UserId);
            
            validationResult.IsUnauthorized = result == null;
            
            return validationResult;
        }
    }
}