using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountCoursePayments
{
    public class GetAccountCoursePaymentsResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}

