using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferRequests
{
    public class GetTransferRequestsResponse
    {
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}