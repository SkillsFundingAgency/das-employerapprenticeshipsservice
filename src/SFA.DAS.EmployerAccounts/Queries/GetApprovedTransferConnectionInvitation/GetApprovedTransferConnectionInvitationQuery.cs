using MediatR;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprovedTransferConnectionInvitation
{
    public class GetApprovedTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetApprovedTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}