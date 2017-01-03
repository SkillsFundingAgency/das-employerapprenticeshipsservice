using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrgainsationValidator : IValidator<GetPublicSectorOrgainsationQuery>
    {
        public ValidationResult Validate(GetPublicSectorOrgainsationQuery item)
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

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetPublicSectorOrgainsationQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
