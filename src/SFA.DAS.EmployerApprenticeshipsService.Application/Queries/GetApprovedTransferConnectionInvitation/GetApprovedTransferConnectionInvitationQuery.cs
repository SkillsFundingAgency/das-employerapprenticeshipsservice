using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetApprovedTransferConnectionInvitation
{
    public class GetApprovedTransferConnectionInvitationQuery : AuthorizedMessage, IAsyncRequest<GetApprovedTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}