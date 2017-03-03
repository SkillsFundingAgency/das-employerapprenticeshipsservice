using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf
{
    public class GetEmployerAgreementPdfValidator : IValidator<GetEmployerAgreementPdfRequest>
    {
        public ValidationResult Validate(GetEmployerAgreementPdfRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.AgreementFileName))
            {
                validationResult.AddError(nameof(item.AgreementFileName));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetEmployerAgreementPdfRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}