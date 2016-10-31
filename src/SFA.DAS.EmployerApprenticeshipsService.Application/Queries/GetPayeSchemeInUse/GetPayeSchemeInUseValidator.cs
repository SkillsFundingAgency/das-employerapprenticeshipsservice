using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse
{
    public class GetPayeSchemeInUseValidator : IValidator<GetPayeSchemeInUseQuery>
    {
        public ValidationResult Validate(GetPayeSchemeInUseQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.Empref))
            {
                validationResult.AddError(nameof(item.Empref),"Empref has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetPayeSchemeInUseQuery item)
        {
            throw new NotImplementedException();
        }
    }
}