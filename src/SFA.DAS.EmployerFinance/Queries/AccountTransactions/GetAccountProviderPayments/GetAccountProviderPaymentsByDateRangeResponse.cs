using SFA.DAS.EmployerFinance.Models.Transaction;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountProviderPayments
{
    public class GetAccountProviderPaymentsByDateRangeResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}