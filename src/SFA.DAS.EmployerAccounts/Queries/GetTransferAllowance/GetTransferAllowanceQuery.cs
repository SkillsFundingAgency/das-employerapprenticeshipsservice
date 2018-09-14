using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQuery : MembershipMessage, IAsyncRequest<GetTransferAllowanceResponse>
    {
    }
}