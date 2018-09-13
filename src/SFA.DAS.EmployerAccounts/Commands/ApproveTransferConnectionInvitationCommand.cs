using MediatR;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class ApproveTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}