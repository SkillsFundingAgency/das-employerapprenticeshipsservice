using System;
using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountLevyTransactions
{
    public class GetAccountLevyTransactionsQuery : IAsyncRequest<GetAccountLevyTransactionsResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
