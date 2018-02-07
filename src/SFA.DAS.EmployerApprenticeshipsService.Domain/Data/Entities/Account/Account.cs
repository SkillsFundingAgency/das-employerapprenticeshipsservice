using System;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Data.Entities.Account
{
    public class Account
    {
        public long Id { get; set; }
        public string HashedId { get; set; }
        public string PublicHashedId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RoleId { get; set; }
        public string RoleName => ((Role)RoleId).ToString();

        public TransferConnectionInvitation SendTransferConnectionInvitation(Account receiverAccount, User senderUser)
        {
            RequiresPendingTransferConnectionInvitationDoesNotAlreadyExist(receiverAccount);
            RequiresTransferConnectionDoesNotAlreadyExist(receiverAccount);

            return new TransferConnectionInvitation(this, receiverAccount, senderUser);
        }

        private void RequiresPendingTransferConnectionInvitationDoesNotAlreadyExist(Account receiverAccount)
        {
            // Rework domain model to allow this invariant to be enforced
        }

        private void RequiresTransferConnectionDoesNotAlreadyExist(Account receiverAccount)
        {
            // Rework domain model to allow this invariant to be enforced
        }
    }
}