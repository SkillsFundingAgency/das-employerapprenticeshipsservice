using System;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesValidator : IValidator<GetAccountLegalEntitiesRequest>
    {
        public ValidationResult Validate(GetAccountLegalEntitiesRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == 0)
            {
                validationResult.AddError(nameof(item.Id), "Account Id has not been supplied");
            }
            if (string.IsNullOrWhiteSpace(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId), "User Id has not been supplied");
            }
            else
            {
                Guid output;
                if (!Guid.TryParse(item.UserId, out output))
                {
                    validationResult.AddError(nameof(item.UserId), "User Id has not been supplied in the correct format");
                }
            }

            return validationResult;
        }
    }
}