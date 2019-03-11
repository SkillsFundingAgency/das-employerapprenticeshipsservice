using SFA.DAS.EmployerAccounts.Models.TransferConnections;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Models.Account
{
    public class Account
    {
        public virtual long Id { get; set; }
        public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; } = new List<AccountLegalEntity>();
        public virtual DateTime CreatedDate { get; set; }
        public virtual string HashedId { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string PublicHashedId { get; set; }
        public virtual Role role { get; set; }
        public string RoleName => (role).ToString();
        public virtual ICollection<TransferConnectionInvitation> ReceivedTransferConnectionInvitations { get; set; } = new List<TransferConnectionInvitation>();
        public virtual ICollection<TransferConnectionInvitation> SentTransferConnectionInvitations { get; set; } = new List<TransferConnectionInvitation>();

        public bool IsTransferConnectionInvitationSender()
        {
            return SentTransferConnectionInvitations.Any(i =>
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved);
        }

        public TransferConnectionInvitation SendTransferConnectionInvitation(Account receiverAccount, User senderUser, decimal senderAccountTransferAllowance)
        {
            RequiresTransferConnectionInvitationSenderIsNotTheReceiver(receiverAccount);
            RequiresMinTransferAllowanceIsAvailable(senderAccountTransferAllowance);
            RequiresTransferConnectionInvitationSenderIsNotAReceiver();
            RequiresTransferConnectionInvitationReceiverIsNotASender(receiverAccount);
            RequiresTransferConnectionInvitationDoesNotExist(receiverAccount);

            var transferConnectionInvitation = new TransferConnectionInvitation(this, receiverAccount, senderUser);

            SentTransferConnectionInvitations.Add(transferConnectionInvitation);

            return transferConnectionInvitation;
        }

        private void RequiresMinTransferAllowanceIsAvailable(decimal senderAccountTransferAllowance)
        {
            if (senderAccountTransferAllowance < Constants.TransferConnectionInvitations.SenderMinTransferAllowance)
                throw new Exception("Requires min transfer allowance is available");
        }

        private void RequiresTransferConnectionInvitationDoesNotExist(Account receiverAccount)
        {
            var anyReceivedTransferConnectionInvitations = ReceivedTransferConnectionInvitations.Any(i =>
                i.SenderAccount.Id == receiverAccount.Id && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            var anySentTransferConnectionInvitations = SentTransferConnectionInvitations.Any(i =>
                i.ReceiverAccount.Id == receiverAccount.Id && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            if (anyReceivedTransferConnectionInvitations || anySentTransferConnectionInvitations)
                throw new Exception("Requires transfer connection invitation does not exist");
        }

        private void RequiresTransferConnectionInvitationReceiverIsNotASender(Account receiverAccount)
        {
            if (receiverAccount.IsTransferConnectionInvitationSender())
                throw new Exception("Requires transfer connection invitation receiver is not a sender");
        }

        private void RequiresTransferConnectionInvitationSenderIsNotTheReceiver(Account receiverAccount)
        {
            if (receiverAccount.Id == Id)
                throw new Exception("Requires transfer connection invitation sender is not the receiver");
        }

        private void RequiresTransferConnectionInvitationSenderIsNotAReceiver()
        {
            var isReceiver = ReceivedTransferConnectionInvitations.Any(i =>
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved);

            if (isReceiver)
                throw new Exception("Requires transfer connection invitation sender is not a receiver");
        }
    }
}