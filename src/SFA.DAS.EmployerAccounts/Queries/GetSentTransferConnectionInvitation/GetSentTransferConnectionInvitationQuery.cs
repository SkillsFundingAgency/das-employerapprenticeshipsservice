using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetSentTransferConnectionInvitation
{
    public class GetSentTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetSentTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}