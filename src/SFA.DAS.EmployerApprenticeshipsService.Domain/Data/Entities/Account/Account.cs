using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Data.Entities.Account
{
    public class Account
    {
        public virtual long Id { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string HashedId { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string PublicHashedId { get; set; }
        public virtual int RoleId { get; set; }
        public string RoleName => ((Role)RoleId).ToString();
        public virtual ICollection<TransferConnectionInvitation> SentTransferConnectionInvitations { get; set; } = new List<TransferConnectionInvitation>();

        public TransferConnectionInvitation SendTransferConnectionInvitation(Account receiverAccount, User senderUser)
        {
            RequiresTransferConnectionInvitationDoesNotAlreadyExist(receiverAccount);

            var transferConnectionInvitation = new TransferConnectionInvitation(this, receiverAccount, senderUser);

            SentTransferConnectionInvitations.Add(transferConnectionInvitation);

            return transferConnectionInvitation;
        }

        private void RequiresTransferConnectionInvitationDoesNotAlreadyExist(Account receiverAccount)
        {
            var anyTransferConnectionInvitations = SentTransferConnectionInvitations.Any(i =>
                i.ReceiverAccount.Id == receiverAccount.Id &&
                i.Status != TransferConnectionInvitationStatus.Rejected);

            if (anyTransferConnectionInvitations)
                throw new Exception("Requires transfer connection invitation does not already exist.");
        }
    }
}