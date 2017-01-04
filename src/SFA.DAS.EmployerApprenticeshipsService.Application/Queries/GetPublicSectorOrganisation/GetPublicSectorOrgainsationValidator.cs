using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrgainsationValidator : IValidator<GetPublicSectorOrganisationQuery>
    {
        public ValidationResult Validate(GetPublicSectorOrganisationQuery item)
        {
            var validationResult = new ValidationResult();

            if (item == null)
            {
                validationResult.ValidationDictionary.Add("Query", "Query should not be null");
                return validationResult;
            }

            if (string.IsNullOrEmpty(item.SearchTerm))
            {
                validationResult.ValidationDictionary.Add("SearchTerm", "Search term has not been supplied");
            }

            if (item.PageNumber < 1)
            {
                validationResult.ValidationDictionary.Add("PageNumber", "Page number must be greater than zero");
            }

            if (item.PageSize < 1)
            {
                validationResult.ValidationDictionary.Add("PageSize", "Page size must be greater than zero");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetPublicSectorOrganisationQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
