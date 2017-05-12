using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountProviderPayments
{
    public class GetAccountProviderPaymentsByDateRangeQuery : IAsyncRequest<GetAccountProviderPaymentsByDateRangeResponse>
    {
        public long AccountId { get; set; }
        public long UkPrn { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}