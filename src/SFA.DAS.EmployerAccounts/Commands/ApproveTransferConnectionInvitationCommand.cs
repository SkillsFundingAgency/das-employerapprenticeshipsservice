using MediatR;
using SFA.DAS.EmployerAccounts.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class ApproveTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}