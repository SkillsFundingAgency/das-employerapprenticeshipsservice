using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Web.ViewModels.Transfers
{
    public class TransferConnectionInvitationsViewModel
    {
        public IEnumerable<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
    }
}