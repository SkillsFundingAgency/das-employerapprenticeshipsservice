using System.Collections.Generic;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.Transfers
{
    public class TransferConnectionInvitationsViewModel : AccountViewModel
    {
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}