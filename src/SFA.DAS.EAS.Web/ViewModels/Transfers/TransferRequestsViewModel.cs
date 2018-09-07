using System.Collections.Generic;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.Transfers
{
    public class TransferRequestsViewModel : AccountViewModel
    {
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}