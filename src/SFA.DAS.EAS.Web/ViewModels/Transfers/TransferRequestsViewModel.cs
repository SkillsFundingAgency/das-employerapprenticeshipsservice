using System.Collections.Generic;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.Transfers
{
    public class TransferRequestsViewModel
    {
        public long AccountId { get; set; }
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}