﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccounts
{
    public class GetEmployerAccountsQuery : IAsyncRequest<GetEmployerAccountsResponse>
    {
        public string ToDate { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
