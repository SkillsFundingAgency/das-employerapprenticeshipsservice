using FluentValidation;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public class SelectProviderViewModelValidator : AbstractValidator<SelectProviderViewModel>
    {
        public SelectProviderViewModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.LegalEntityCode).NotEmpty();

            RuleFor(x => x.NotFound).Cascade(CascadeMode.StopOnFirstFailure).Equal(false)
                .WithMessage("Check UK Provider Reference Number");

            RuleFor(x => x.ProviderId)
                .NotEmpty().WithMessage("Check UK Provider Reference Number")
                .Matches("^[0-9]{8}[\\s]*$").WithMessage("Check UK Provider Reference Number")
                .When(x => x.NotFound == false); 
        }
    }
}