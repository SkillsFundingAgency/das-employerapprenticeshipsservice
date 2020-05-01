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

            var isValidType = Enum.TryParse(item.ContentType,true, out ContentType type);
            
            if (string.IsNullOrEmpty(item.ContentType))
            {
                validationResult.AddError(nameof(item.ContentType), "Type has not been supplied");
            }
            if (!isValidType)
            {
                validationResult.AddError(nameof(item.ContentType), "Not a valid ContentType");
            }
            if (string.IsNullOrEmpty(item.ClientId))
            {
                validationResult.AddError(nameof(item.ClientId), "ClientId has not been supplied");
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetClientContentRequest item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
