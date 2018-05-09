using MediatR;
using System;

namespace SFA.DAS.EAS.Application.Queries.FindAccountCoursePayments
{
    public class FindAccountCoursePaymentsQuery : IAsyncRequest<FindAccountCoursePaymentsResponse>
    {
        public string HashedAccountId { get; set; }
        public long UkPrn { get; set; }
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
