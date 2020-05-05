using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetClientContent
{
    public class GetClientContentRequestValidator : IValidator<GetClientContentRequest>
    {
        public ValidationResult Validate(GetClientContentRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.ContentType))
            {
                validationResult.AddError(nameof(item.ContentType), "Type has not been supplied");
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetClientContentRequest item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
