using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountLevyDeclationTransactionsByDateRangeResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}