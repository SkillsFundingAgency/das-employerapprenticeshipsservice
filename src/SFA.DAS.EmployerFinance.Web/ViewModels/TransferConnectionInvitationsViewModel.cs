using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerFinance.Dtos;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransferConnectionInvitationsViewModel : AccountViewModel
    {
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}