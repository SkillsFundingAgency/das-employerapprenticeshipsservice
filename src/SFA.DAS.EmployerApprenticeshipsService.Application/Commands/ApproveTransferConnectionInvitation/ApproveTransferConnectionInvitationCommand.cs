using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation
{
    public class ApproveTransferConnectionInvitationCommand : AuthorizedMessage, IAsyncRequest
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}