using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerFinance.Dtos;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransferRequestsViewModel : AccountViewModel
    {
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}