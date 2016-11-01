using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    public class SelectProviderViewModelValidator : AbstractValidator<SelectProviderViewModel>
    {
        public SelectProviderViewModelValidator()
        {
            RuleFor(x => x.ProviderId).NotEmpty().WithMessage("This is a test").Matches("^$^[0-9]{8}*$").WithMessage("Enter a valid UK Provider Reference Number");
        }
    }
}