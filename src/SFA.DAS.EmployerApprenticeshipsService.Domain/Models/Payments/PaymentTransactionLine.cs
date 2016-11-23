using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Models.Payments
{
    public class PaymentTransactionLine : TransactionLine
    {
        public Guid? PaymentId { get; set; }
        public long UkPrn { get; set; }
        public string PeriodEnd { get; set; }
        public string ProviderName { get; set; }
        public decimal LineAmount { get; set; }
        public ICollection<PaymentTransactionLine> SubPayments =>
            SubTransactions?.OfType<PaymentTransactionLine>().ToList() ??
                new List<PaymentTransactionLine>();
    }
}
