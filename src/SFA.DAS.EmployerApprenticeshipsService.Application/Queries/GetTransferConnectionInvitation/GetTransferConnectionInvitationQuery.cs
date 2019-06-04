using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}