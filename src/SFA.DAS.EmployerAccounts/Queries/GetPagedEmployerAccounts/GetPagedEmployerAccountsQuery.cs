﻿using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts
{
    public class GetPagedEmployerAccountsQuery : IAsyncRequest<GetPagedEmployerAccountsResponse>
    {
        public string ToDate { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
