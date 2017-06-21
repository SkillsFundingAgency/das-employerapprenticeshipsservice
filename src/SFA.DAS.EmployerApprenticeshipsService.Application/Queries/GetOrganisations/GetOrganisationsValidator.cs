using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisations
{
    public class GetOrganisationsValidator : IValidator<GetOrganisationsRequest>
    {
        public ValidationResult Validate(GetOrganisationsRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.SearchTerm))
            {
                validationResult.AddError(nameof(item.SearchTerm));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetOrganisationsRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}