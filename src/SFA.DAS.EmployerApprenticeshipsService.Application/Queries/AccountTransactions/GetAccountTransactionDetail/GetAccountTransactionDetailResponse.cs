using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountTransactionDetailResponse
    {
        public List<TransactionLineDetail> Data { get; set; }
    }
}