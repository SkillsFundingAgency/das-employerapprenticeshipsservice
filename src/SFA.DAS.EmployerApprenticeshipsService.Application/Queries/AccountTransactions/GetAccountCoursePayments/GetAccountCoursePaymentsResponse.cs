using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountCoursePayments
{
    public class GetAccountCoursePaymentsResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}

