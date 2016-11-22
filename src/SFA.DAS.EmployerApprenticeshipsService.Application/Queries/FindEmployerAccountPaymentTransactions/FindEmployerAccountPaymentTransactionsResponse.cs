using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions
{
    public class FindEmployerAccountPaymentTransactionsResponse
    {
        public string ProviderName { get; set; }
        public List<PaymentTransactionLine> Transactions { get; set; }
        public decimal Total { get; set; }
    }
}