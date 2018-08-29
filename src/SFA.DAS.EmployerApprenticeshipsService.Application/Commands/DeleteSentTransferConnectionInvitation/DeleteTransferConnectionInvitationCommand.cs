using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Commands.DeleteSentTransferConnectionInvitation
{
    public class DeleteTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}