using MediatR;
using SFA.DAS.EmployerAccounts.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class DeleteTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}