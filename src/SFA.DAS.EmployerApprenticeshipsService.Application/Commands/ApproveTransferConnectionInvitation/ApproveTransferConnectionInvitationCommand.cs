using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation
{
    public class ApproveTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}