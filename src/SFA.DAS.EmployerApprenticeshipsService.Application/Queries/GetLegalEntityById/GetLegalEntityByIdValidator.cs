using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdValidator : IValidator<GetLegalEntityByIdQuery>
    {
        public ValidationResult Validate(GetLegalEntityByIdQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == 0)
            {
                validationResult.AddError(nameof(item.Id), "LegalEntityId has not been supplied");
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetLegalEntityByIdQuery item)
        {
            throw new NotImplementedException();
        }
    }
}