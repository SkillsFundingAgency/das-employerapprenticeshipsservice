using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequest : IAsyncRequest<GetPreviousTransactionsCountResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }

        public string ExternalUserId { get; set; }
    }
}
