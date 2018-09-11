using SFA.DAS.EmployerFinance.Dtos;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferRequests
{
    public class GetTransferRequestsResponse
    {
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}