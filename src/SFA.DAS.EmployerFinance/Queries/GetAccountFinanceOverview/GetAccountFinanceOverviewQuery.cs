using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewQuery : MembershipMessage, IAsyncRequest<GetAccountFinanceOverviewResponse>
    {

    }
}
