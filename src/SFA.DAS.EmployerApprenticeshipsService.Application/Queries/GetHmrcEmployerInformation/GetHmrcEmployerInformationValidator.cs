using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationValidator : IValidator<GetHmrcEmployerInformationQuery>
    {
        public ValidationResult Validate(GetHmrcEmployerInformationQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.AuthToken))
            {
                validationResult.AddError(nameof(item.AuthToken),"AuthToken has not been supplied");
            }
            
            return validationResult;
        }
    }
}