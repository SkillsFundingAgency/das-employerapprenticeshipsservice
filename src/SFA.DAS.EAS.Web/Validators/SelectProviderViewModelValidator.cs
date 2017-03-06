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
                .WithMessage("Training Provider not found");

            RuleFor(x => x.ProviderId)
                .NotEmpty().WithMessage("Enter a valid UK Provider Reference Number")
                .Matches("^[0-9]{8}[\\s]*$").WithMessage("Enter a valid UK Provider Reference Number")
                .When(x => x.NotFound == false); 
        }
    }
}