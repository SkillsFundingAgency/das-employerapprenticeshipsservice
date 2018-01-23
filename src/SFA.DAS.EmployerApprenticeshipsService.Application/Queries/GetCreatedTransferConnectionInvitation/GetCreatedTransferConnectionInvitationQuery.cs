using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnectionInvitation
{
    public class GetCreatedTransferConnectionInvitationQuery : IAsyncRequest<GetCreatedTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}