using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountCourseTransactions
{
    public class GetAccountCourseTransactionsQuery : IAsyncRequest<GetAccountCourseTransactionsResponse>
    {
        public long AccountId { get; set; }
        public long UkPrn { get; set; }
        public string CourseName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
