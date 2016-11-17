using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailResponse
    {
        public List<TransactionDetailSummary> TransactionDetail { get; set; }
        public decimal Total { get; set; }
    }
}