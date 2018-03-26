using System.Collections.Generic;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetTransferRequests
{
    public class GetTransferRequestsResponse
    {
        public long AccountId { get; set; }
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}