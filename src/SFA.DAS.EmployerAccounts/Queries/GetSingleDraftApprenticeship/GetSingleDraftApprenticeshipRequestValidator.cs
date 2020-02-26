using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleDraftApprenticeship
{
    public class GetSingleDraftApprenticeshipRequestValidator : IValidator<GetSingleDraftApprenticeshipRequest>
    {
        public ValidationResult Validate(GetSingleDraftApprenticeshipRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.CohortId == null)
            {
                validationResult.ValidationDictionary.Add(nameof(item.CohortId),
                    "CohortId cannot be null or empty.");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetSingleDraftApprenticeshipRequest item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
