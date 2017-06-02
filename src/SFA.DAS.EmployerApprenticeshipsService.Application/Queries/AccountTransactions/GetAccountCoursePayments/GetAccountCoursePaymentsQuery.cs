using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountCoursePayments
{
    public class GetAccountCoursePaymentsQuery : IAsyncRequest<GetAccountCoursePaymentsResponse>
    {
        public long AccountId { get; set; }
        public long UkPrn { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
