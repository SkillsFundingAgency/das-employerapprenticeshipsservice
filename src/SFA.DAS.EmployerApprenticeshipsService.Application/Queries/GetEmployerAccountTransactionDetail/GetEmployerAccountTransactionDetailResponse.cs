using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailResponse
    {
        public List<TransactionDetailSummary> TransactionDetail { get; set; }
        public decimal Total { get; set; }
    }
}