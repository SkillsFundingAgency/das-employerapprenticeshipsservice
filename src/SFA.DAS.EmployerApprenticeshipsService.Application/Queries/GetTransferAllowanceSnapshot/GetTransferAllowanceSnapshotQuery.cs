using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowanceSnapshot
{
    public class GetTransferAllowanceSnapshotQuery : MembershipMessage, IAsyncRequest<GetTransferAllowanceSnapshotResponse>
    {
        public short ?Year { get; set; }
    }
}