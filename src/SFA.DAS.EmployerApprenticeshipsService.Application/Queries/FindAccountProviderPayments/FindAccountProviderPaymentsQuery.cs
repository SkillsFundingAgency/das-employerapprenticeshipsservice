using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments
{
    public class FindAccountProviderPaymentsQuery : IAsyncRequest<FindAccountProviderPaymentsResponse>
    {
        public string HashedAccountId { get; set; }
        public long UkPrn { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
