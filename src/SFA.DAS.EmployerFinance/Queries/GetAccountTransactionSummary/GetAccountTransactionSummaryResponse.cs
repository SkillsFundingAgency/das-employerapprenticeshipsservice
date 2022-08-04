using SFA.DAS.EAS.Finance.Api.Types;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary
{
    public class GetAccountTransactionSummaryResponse
    {
        public List<TransactionSummary> Data { get; set; }
    }
}
