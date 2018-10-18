using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class PaymentTransactionViewModel : TransactionLineViewModel<PaymentTransactionLine>
    {
        public string ProviderName { get; set; }

        public ICollection<ApprenticeshipPaymentGroup> CoursePaymentGroups { get; set; }
    }
}