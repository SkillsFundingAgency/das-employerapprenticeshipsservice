using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions
{
    public class GetAccountTransactionsResponse
    {
        public List<TransactionLine> TransactionLines { get; set; }
    }
}