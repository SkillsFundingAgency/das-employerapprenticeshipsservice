using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}