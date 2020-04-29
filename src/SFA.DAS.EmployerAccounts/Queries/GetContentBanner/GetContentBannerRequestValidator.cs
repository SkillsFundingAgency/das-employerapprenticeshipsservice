using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetContentBanner
{
    public class GetContentBannerRequestValidator : IValidator<GetContentBannerRequest>
    {
        public ValidationResult Validate(GetContentBannerRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.BannerId == 0)
            {
                validationResult.AddError(nameof(item.BannerId), "BannerId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetContentBannerRequest item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
