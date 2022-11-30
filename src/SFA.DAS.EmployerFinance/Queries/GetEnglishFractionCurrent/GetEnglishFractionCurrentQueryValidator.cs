using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFractionCurrent
{
    public class GetEnglishFractionCurrentQueryValidator : IValidator<GetEnglishFractionCurrentQuery>
    {
        public ValidationResult Validate(GetEnglishFractionCurrentQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (item.EmpRefs == null || item.EmpRefs.Any(p => string.IsNullOrEmpty(p)))
            {
                validationResult.AddError(nameof(item.EmpRefs));
            }
           
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetEnglishFractionCurrentQuery item)
        {
            throw new NotImplementedException();
        }
    }
}