using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountTransactionDetailResponse
    {
        public List<TransactionLineDetail> Data { get; set; }
    }
}