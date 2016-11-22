using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions
{
    public class FindEmployerAccountPaymentTransactionsQuery : IAsyncRequest<FindEmployerAccountPaymentTransactionsResponse>
    {
        public string HashedAccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
