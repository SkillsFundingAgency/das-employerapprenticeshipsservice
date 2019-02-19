using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds
{
    public class GetAccountFinanceOverviewQuery : MembershipMessage, IAsyncRequest<GetAccountFinanceOverviewResponse>
    {
      
    }
}
