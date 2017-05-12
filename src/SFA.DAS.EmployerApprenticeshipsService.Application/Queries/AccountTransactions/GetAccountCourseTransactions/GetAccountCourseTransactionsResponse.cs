using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountCourseTransactions
{
    public class GetAccountCourseTransactionsResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}

