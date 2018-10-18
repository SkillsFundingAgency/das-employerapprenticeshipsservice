using System.Linq;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class TransferConnectionInvitationDtoExtensions
    {
        public static TransferConnectionInvitationChangeDto GetApprovedChange(this TransferConnectionInvitationDto transferConnectionInvitation)
        {
            return transferConnectionInvitation.Changes.Single(c => c.Status == TransferConnectionInvitationStatus.Approved);
        }

        public static AccountDto GetPeerAccount(this TransferConnectionInvitationDto transferConnectionInvitation, long accountId)
        {
            return transferConnectionInvitation.SenderAccount.Id == accountId
                ? transferConnectionInvitation.ReceiverAccount
                : transferConnectionInvitation.SenderAccount;
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