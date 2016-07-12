using System;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryValidator : IValidator<GetLevyDeclarationQuery>
    {
        public ValidationResult Validate(GetLevyDeclarationQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Id))
            {
                validationResult.AddError(nameof(item.Id), "The Id field has not been supplied");    
            }

            return validationResult;
        }
    }
}