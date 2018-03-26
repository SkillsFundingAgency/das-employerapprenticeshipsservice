using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.TransferRequests;
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
        public virtual ICollection<TransferConnectionInvitation> ReceivedTransferConnectionInvitation { get; set; } = new List<TransferConnectionInvitation>();
        public bool IsSender => SentTransferConnectionInvitations.Any(i =>i.Status != TransferConnectionInvitationStatus.Rejected);

        public TransferConnectionInvitation SendTransferConnectionInvitation(Account receiverAccount, User senderUser)
        {
            RequiresReceiverAccountIsNotAlreadyASender(receiverAccount);
            RequiresReceiverAccountIsNotTheSenderAccount(receiverAccount);
            RequiresTransferConnectionInvitationDoesNotAlreadyExist(receiverAccount);

            var transferConnectionInvitation = new TransferConnectionInvitation(this, receiverAccount, senderUser);

            SentTransferConnectionInvitations.Add(transferConnectionInvitation);

            return transferConnectionInvitation;
        }
        
        public TransferRequest SentTransferRequest(Account senderAccount, long commitmentId, string commitmentHashedId, decimal transferCost)
        {
            return new TransferRequest(commitmentId, commitmentHashedId, this, senderAccount, transferCost);
        }

        private void RequiresReceiverAccountIsNotAlreadyASender(Account receiverAccount)
        {
            if (receiverAccount.IsSender)
                throw new Exception("Requires receiver account is not already a sender.");
        }

        private void RequiresReceiverAccountIsNotTheSenderAccount(Account receiverAccount)
        {
            if (receiverAccount.Id == Id)
                throw new Exception("Requires receiver account is not the sender account.");
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