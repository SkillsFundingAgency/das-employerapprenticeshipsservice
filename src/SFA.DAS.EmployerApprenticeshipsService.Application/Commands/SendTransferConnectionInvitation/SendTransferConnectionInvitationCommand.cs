using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommand : IAsyncRequest
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}