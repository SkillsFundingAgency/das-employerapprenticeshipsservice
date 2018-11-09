using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountBalances
{
    public class GetAccountBalancesRequest :IAsyncRequest<GetAccountBalancesResponse>
    {
        public List<long> AccountIds { get; set; }
    }
}
