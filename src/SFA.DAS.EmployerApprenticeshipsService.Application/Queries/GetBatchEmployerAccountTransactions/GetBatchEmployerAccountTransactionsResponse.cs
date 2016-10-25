using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetBatchEmployerAccountTransactions
{
    public class GetBatchEmployerAccountTransactionsResponse
    {
        public List<AggregationData> Data { get; set; }
    }
}