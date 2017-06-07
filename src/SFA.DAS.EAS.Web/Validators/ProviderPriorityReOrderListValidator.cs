using FluentValidation;
using SFA.DAS.EAS.Web.ViewModels;
using System.Linq;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class ProviderPriorityReOrderListValidator : AbstractValidator<ProviderPriorityReOrderViewModel>
    {
        public ProviderPriorityReOrderListValidator()
        {
            RuleFor(x => x.Priorities)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Must(x =>
                {
                    var hasDuplicates = x.GroupBy(y => y).Any(g => g.Count() > 1);
                    return !hasDuplicates;
                }).WithMessage("Set payment order");
        }
    }
}