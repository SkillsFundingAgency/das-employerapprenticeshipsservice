using System.Linq;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class TransferConnectionInvitationDtoExtensions
    {
        public static TransferConnectionInvitationChangeDto GetApprovedChange(this TransferConnectionInvitationDto transferConnectionInvitation)
        {
            return transferConnectionInvitation.Changes.Single(c => c.Status == TransferConnectionInvitationStatus.Approved);
        }

        public static TransferConnectionInvitationChangeDto GetPendingChange(this TransferConnectionInvitationDto transferConnectionInvitation)
        {
            return transferConnectionInvitation.Changes.Single(c => c.Status == TransferConnectionInvitationStatus.Pending);
        }

        public static TransferConnectionInvitationChangeDto GetRejectedChange(this TransferConnectionInvitationDto transferConnectionInvitation)
        {
            return transferConnectionInvitation.Changes.Single(c => c.Status == TransferConnectionInvitationStatus.Rejected);
        }
    }
}