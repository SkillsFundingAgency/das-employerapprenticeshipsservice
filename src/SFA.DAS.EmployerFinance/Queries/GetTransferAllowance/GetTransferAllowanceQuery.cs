using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQuery : MembershipMessage, IAsyncRequest<GetTransferAllowanceResponse>
    {
    }
}