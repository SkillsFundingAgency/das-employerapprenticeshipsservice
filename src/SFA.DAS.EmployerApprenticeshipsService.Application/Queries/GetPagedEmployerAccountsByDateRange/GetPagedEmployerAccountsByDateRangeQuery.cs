using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange
{
    public class GetPagedEmployerAccountsByDateRangeQuery : IAsyncRequest<GetPagedEmployerAccountsByDateRangeResponse>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}