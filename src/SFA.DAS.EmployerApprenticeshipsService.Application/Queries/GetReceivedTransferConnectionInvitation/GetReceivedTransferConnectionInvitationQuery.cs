using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation
{
    public class GetReceivedTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetReceivedTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}