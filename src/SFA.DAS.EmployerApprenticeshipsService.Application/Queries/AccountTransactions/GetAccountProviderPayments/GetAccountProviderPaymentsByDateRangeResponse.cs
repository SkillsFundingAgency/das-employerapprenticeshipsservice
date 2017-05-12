using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountProviderPayments
{
    public class GetAccountProviderPaymentsByDateRangeResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}