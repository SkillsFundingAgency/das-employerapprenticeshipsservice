using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery
{
    public class GetSentTransferConnectionQuery : IAsyncRequest<SentTransferConnectionViewModel>
    {
        [Required]
        public long? TransferConnectionId { get; set; }
    }
}