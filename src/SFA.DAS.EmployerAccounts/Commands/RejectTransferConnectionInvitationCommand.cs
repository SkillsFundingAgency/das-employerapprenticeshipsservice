using MediatR;
using SFA.DAS.EmployerAccounts.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class RejectTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}