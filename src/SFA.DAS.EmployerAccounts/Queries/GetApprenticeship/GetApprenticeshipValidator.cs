using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipValidator : IValidator<GetApprenticeshipRequest>
    {
        public ValidationResult Validate(GetApprenticeshipRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId is null)
            {
                validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetApprenticeshipRequest item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
