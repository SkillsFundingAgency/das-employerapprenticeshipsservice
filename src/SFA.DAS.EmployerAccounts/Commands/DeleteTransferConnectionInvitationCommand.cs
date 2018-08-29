using MediatR;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class DeleteTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}