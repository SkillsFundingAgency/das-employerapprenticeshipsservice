using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferRequestsViewModel : DAS.Authorization.Mvc.AccountViewModel
    {
        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}