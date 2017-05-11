using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments
{
    public class FindAccountProviderPaymentsResponse
    {
        public string ProviderName { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<PaymentTransactionLine> Transactions { get; set; }
        public decimal Total { get; set; }
    }
}