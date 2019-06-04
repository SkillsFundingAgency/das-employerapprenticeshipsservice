using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQuery : MembershipMessage, IAsyncRequest<GetTransferAllowanceResponse>
    {
    }
}