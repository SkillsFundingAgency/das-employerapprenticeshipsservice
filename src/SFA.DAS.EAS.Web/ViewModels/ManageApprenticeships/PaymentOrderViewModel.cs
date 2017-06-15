using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class PaymentOrderViewModel : ViewModelBase
    {
        public IEnumerable<PaymentOrderItem> Items { get; set; }
    }

    public class PaymentOrderItem
    {
        public string ProviderName { get; set; }

        public long ProviderId { get; set; }

        public int Priority { get; set; }
    }
}