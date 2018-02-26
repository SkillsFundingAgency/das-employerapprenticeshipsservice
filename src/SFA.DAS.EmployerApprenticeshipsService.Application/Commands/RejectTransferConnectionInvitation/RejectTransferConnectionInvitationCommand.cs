using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Commands.RejectTransferConnectionInvitation
{
    public class RejectTransferConnectionInvitationCommand : AuthorizedMessage, IAsyncRequest
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}