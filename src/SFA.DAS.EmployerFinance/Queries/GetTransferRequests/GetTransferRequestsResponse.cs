using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferRequests
{
    public class GetTransferRequestsResponse
    {
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
        public long AccountId { get; set; }
    }
}