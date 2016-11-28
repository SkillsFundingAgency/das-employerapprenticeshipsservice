using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Web.Models
{
    public class PaymentTransactionViewModel : TransactionLineViewModel<PaymentTransactionLine>
    {
        public string ProviderName { get; set; }
    }
}