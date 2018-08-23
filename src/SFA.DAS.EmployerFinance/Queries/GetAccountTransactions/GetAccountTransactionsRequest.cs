using System;
using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactions
{
    public class GetAccountTransactionsRequest : IAsyncRequest<GetAccountTransactionsResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
