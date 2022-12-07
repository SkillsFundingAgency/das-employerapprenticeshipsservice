using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFrationHistory
{
    public class GetEnglishFractionHistoryQueryValidator : IValidator<GetEnglishFractionHistoryQuery>
    {
        public ValidationResult Validate(GetEnglishFractionHistoryQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (string.IsNullOrEmpty(item.EmpRef))
            {
                validationResult.AddError(nameof(item.EmpRef));
            }
           
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetEnglishFractionHistoryQuery item)
        {
            throw new NotImplementedException();
        }
    }
}