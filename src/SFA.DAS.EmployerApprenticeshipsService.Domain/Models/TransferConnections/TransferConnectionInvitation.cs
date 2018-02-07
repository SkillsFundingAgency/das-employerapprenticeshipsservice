using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Domain.Models.TransferConnections
{
    public class TransferConnectionInvitation : Entity
    {
        public long Id { get; set; }
        public long SenderUserId { get; set; }
        public long SenderAccountId { get; set; }
        public long ReceiverAccountId { get; set; }
        public long? ApproverUserId { get; set; }
        public long? RejectorUserId { get; set; }
        public TransferConnectionInvitationStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual User SenderUser { get; set; }
        public virtual Data.Entities.Account.Account SenderAccount { get; set; }
        public virtual Data.Entities.Account.Account ReceiverAccount { get; set; }
        public virtual User ApproverUser { get; set; }
        public virtual User RejectorUser { get; set; }

        public TransferConnectionInvitation(Data.Entities.Account.Account senderAccount, Data.Entities.Account.Account receiverAccount, User senderUser)
        {
            SenderUser = senderUser;
            SenderAccount = senderAccount;
            ReceiverAccount = receiverAccount;
            Status = TransferConnectionInvitationStatus.Pending;
            CreatedDate = DateTime.UtcNow;

            Publish<TransferConnectionInvitationSentMessage>(e =>
            {
                e.TransferConnectionId = Id;
                e.TransferConnectionInvitationId = Id;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.CreatorUserRef = SenderUser.ExternalId.ToString();
                e.SenderUserExternalId = SenderUser.ExternalId;
                e.CreatorName = SenderUser.FullName;
                e.SenderUserName = SenderUser.FullName;
                e.CreatedAt = CreatedDate;
            });
        }

        public TransferConnectionInvitation()
        {
        }

        public void Approve(Data.Entities.Account.Account approverAccount, User approverUser)
        {
            RequiresApproverAccountIsTheReceiverAccount(approverAccount);
            RequiresTransferConnectionInvitationIsPending();

            ApproverUser = approverUser;
            Status = TransferConnectionInvitationStatus.Approved;
            ModifiedDate = DateTime.UtcNow;

            Publish<TransferConnectionInvitationApprovedMessage>(e =>
            {
                e.TransferConnectionInvitationId = Id;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.ApproverUserExternalId = ApproverUser.ExternalId;
                e.ApproverUserName = ApproverUser.FullName;
                e.CreatedAt = ModifiedDate.Value;
            });
        }

        public void Reject(Data.Entities.Account.Account rejectorAccount, User rejectorUser)
        {
            RequiresRejectorAccountIsTheReceiverAccount(rejectorAccount);
            RequiresTransferConnectionInvitationIsPending();

            RejectorUser = rejectorUser;
            Status = TransferConnectionInvitationStatus.Rejected;
            ModifiedDate = DateTime.UtcNow;

            Publish<TransferConnectionInvitationRejectedMessage>(e =>
            {
                e.TransferConnectionInvitationId = Id;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.RejectorUserExternalId = RejectorUser.ExternalId;
                e.RejectorUserName = RejectorUser.FullName;
                e.CreatedAt = ModifiedDate.Value;
            });
        }

        private void RequiresApproverAccountIsTheReceiverAccount(Data.Entities.Account.Account approverAccount)
        {
            if (approverAccount.Id != ReceiverAccount.Id)
            {
                throw new Exception("Requires approver account is the receiver account.");
            }
        }

        private void RequiresRejectorAccountIsTheReceiverAccount(Data.Entities.Account.Account rejectorAccount)
        {
            if (rejectorAccount.Id != ReceiverAccount.Id)
            {
                throw new Exception("Requires rejector account is the receiver account.");
            }
        }

        private void RequiresTransferConnectionInvitationIsPending()
        {
            if (Status != TransferConnectionInvitationStatus.Pending)
            {
                throw new Exception("Requires transfer connection invitation is pending.");
            }
        }
    }
}