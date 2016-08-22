using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQueryValidator : IValidator<GetHMRCLevyDeclarationQuery>
    {
        public ValidationResult Validate(GetHMRCLevyDeclarationQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Id))
            {
                validationResult.AddError(nameof(item.Id), "The Id field has not been supplied");    
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetHMRCLevyDeclarationQuery item)
        {
            throw new NotImplementedException();
        }
    }
}