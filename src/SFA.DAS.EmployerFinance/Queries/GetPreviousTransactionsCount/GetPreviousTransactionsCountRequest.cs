using System;
using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequest : IAsyncRequest<GetPreviousTransactionsCountResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }
    }
}
