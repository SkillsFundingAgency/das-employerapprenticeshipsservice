using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation
{
    public class GetSentTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetSentTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}