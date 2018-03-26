using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQuery : MembershipMessage, IAsyncRequest<GetTransferAllowanceResponse>
    {
    }
}