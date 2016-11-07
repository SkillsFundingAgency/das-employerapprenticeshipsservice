using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailResponse
    {
        public List<TransactionLineDetail> TransactionDetail { get; set; }
    }
}