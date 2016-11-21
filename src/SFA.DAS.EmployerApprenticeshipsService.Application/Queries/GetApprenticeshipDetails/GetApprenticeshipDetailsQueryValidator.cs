using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipDetails
{
    public class GetApprenticeshipDetailsQueryValidator : IValidator<GetApprenticeshipDetailsQuery>
    {
        public ValidationResult Validate(GetApprenticeshipDetailsQuery query)
        {
            var result = new ValidationResult();

            if (query.ProviderId < 1)
            {
                result.AddError(nameof(query.ProviderId), "Provider ID has not been supplied");
            }

            if (query.ApprenticeshipId < 1)
            {
                result.AddError(nameof(query.ApprenticeshipId), "Apprenticeship ID has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetApprenticeshipDetailsQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
