using SFA.DAS.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Queries.GetCohorts
{
    public class GetCohortsRequestValidator : IValidator<GetCohortsRequest>
    {
        public ValidationResult Validate(GetCohortsRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId is null)
            {
                validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetCohortsRequest item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
