using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation
{
    public class GetReceivedTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetReceivedTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}