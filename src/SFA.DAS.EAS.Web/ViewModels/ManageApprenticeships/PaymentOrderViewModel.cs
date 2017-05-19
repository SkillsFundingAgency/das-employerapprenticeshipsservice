using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class PaymentOrderViewModel
    {
        public IEnumerable<PaymentOrderItem> PaymentOrderItems { get; set; }
    }

    public class PaymentOrderItem
    {
        public string ProviderName { get; set; }

        public long ProviderId { get; set; }

        public int InitialOrder { get; set; }

        public int NewOrder { get; set; }
    }
}