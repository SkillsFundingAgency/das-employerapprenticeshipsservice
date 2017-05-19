using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation;

using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class UpdateApprenticeshipViewModelValidator : AbstractValidator<UpdateApprenticeshipViewModel>
    {
        public UpdateApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotNull().WithMessage("Select an option");
        }
    }

    public sealed class PaymentOrderViewModelValidator : AbstractValidator<PaymentOrderViewModel>
    {
        public PaymentOrderViewModelValidator()
        {
            RuleFor(x => x.Items)
                .Must(ValidateModel).WithMessage("Input data is not valid");
        }

        private bool ValidateModel(IEnumerable<PaymentOrderItem> items)
        {
            if (items.Any(m => m.NewOrder < 1 || m.NewOrder > items.Count()))
            {
                return false;
            }

            if (items.DistinctBy(m => m.NewOrder).Count() != items.Count())
                return false;

            return true;
        }
    }
}