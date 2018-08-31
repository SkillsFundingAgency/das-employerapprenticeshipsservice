using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountLevyTransactions
{
    /// <summary>
    ///  AML-2454: Move to finance
    /// </summary>
    public class GetAccountLevyTransactionsQuery : IAsyncRequest<GetAccountLevyTransactionsResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
