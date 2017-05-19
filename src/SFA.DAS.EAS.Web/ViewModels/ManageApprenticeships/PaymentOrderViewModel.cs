using System.Collections.Generic;

using FluentValidation.Attributes;

using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(PaymentOrderViewModelValidator))]
    public class PaymentOrderViewModel : ViewModelBase
    {
        public IEnumerable<PaymentOrderItem> Items { get; set; }

        public string ChangesConfirmedError => GetErrorMessage(nameof(Items));
    }

    public class PaymentOrderItem
    {
        public string ProviderName { get; set; }

        public long ProviderId { get; set; }

        public int InitialOrder { get; set; }

        public int NewOrder { get; set; }
    }
}