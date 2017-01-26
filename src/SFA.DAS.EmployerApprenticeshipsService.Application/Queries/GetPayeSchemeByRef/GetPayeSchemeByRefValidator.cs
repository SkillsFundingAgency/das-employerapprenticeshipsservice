using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef
{
    public class GetPayeSchemeByRefValidator : IValidator<GetPayeSchemeByRefQuery>
    {
        public ValidationResult Validate(GetPayeSchemeByRefQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Ref))
            {
                validationResult.AddError(nameof(item.Ref));
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetPayeSchemeByRefQuery item)
        {
            throw new NotImplementedException();
        }
    }
}