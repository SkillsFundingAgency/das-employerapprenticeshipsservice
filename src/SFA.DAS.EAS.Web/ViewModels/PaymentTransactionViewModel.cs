using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class PaymentTransactionViewModel : TransactionLineViewModel<PaymentTransactionLine>
    {
        public string ProviderName { get; set; }

        public ICollection<ApprenticeshipPaymentGroup> CoursePaymentGroups { get; set; }
    }
}