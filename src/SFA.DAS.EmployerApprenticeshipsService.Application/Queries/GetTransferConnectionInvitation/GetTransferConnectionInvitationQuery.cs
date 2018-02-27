using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQuery : AuthorizedMessage, IAsyncRequest<GetTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}