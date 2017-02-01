using FluentValidation;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public class SelectProviderViewModelValidator : AbstractValidator<SelectProviderViewModel>
    {
        public SelectProviderViewModelValidator()
        {
            RuleFor(x => x.LegalEntityCode).NotEmpty();
            RuleFor(x => x.ProviderId)
                .NotEmpty().WithMessage("Enter a valid UK Provider Reference Number")
                .Matches("^[0-9]{8}$").WithMessage("Enter a valid UK Provider Reference Number");
        }
    }
}