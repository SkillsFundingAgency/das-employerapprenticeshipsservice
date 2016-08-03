using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationValidator : IValidator<GetHmrcEmployerInformatioQuery>
    {
        public ValidationResult Validate(GetHmrcEmployerInformatioQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.AuthToken))
            {
                validationResult.AddError(nameof(item.AuthToken),"AuthToken has not been supplied");
            }

            if (string.IsNullOrWhiteSpace(item.Empref))
            {
                validationResult.AddError(nameof(item.Empref), "Empref has not been supplied");
            }

            return validationResult;
        }
    }
}