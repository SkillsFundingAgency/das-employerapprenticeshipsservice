﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances
{
    public class GetAccountBalancesRequest :IAsyncRequest<GetAccountBalancesResponse>
    {
        public List<long> AccountIds { get; set; }
    }
}
