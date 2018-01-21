using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnection
{
    public class GetCreatedTransferConnectionQuery : IAsyncRequest<CreatedTransferConnectionViewModel>
    {
        [Required]
        public long? TransferConnectionId { get; set; }
    }
}