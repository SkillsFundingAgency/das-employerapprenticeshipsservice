using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferConnectionInvitationsViewModel : DAS.Authorization.Mvc.AccountViewModel
    {
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}