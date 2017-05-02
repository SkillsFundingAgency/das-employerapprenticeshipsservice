using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsQuery : IAsyncRequest<GetEmployerAccountTransactionsResponse>
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }


    }
}
