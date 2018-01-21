using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnection
{
    public class SendTransferConnectionCommand : IAsyncRequest
    {
        [Required]
        public long? TransferConnectionId { get; set; }
    }
}