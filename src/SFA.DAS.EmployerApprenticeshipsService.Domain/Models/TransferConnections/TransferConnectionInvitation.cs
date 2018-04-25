﻿using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Domain.Models.TransferConnections
{
    public class TransferConnectionInvitation : Entity
    {
        public virtual int Id { get; protected set; }
        public virtual ICollection<TransferConnectionInvitationChange> Changes { get; protected set; } = new List<TransferConnectionInvitationChange>();
        public virtual DateTime CreatedDate { get; protected set; }
        public virtual bool DeletedByReceiver { get; protected set; }
        public virtual bool DeletedBySender { get; protected set; }
        public virtual Account.Account ReceiverAccount { get; protected set; }
        public virtual long ReceiverAccountId { get; protected set; }
        public virtual Account.Account SenderAccount { get; protected set; }
        public virtual long SenderAccountId { get; protected set; }
        public virtual TransferConnectionInvitationStatus Status { get; protected set; }

        public TransferConnectionInvitation(Account.Account senderAccount, Account.Account receiverAccount, User senderUser) : this()
        {
            var now = DateTime.UtcNow;

            SenderAccount = senderAccount;
            ReceiverAccount = receiverAccount;
            Status = TransferConnectionInvitationStatus.Pending;
            CreatedDate = now;
            
            Changes.Add(new TransferConnectionInvitationChange
            {
                SenderAccount = SenderAccount,
                ReceiverAccount = ReceiverAccount,
                Status = Status,
                DeletedBySender = DeletedBySender,
                DeletedByReceiver = DeletedByReceiver,
                User = senderUser,
                CreatedDate = now
            });

            Publish<SentTransferConnectionInvitationEvent>(e =>
            {
                e.CreatedAt = now;
                e.ReceiverAccountHashedId = ReceiverAccount.HashedId;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.SenderAccountHashedId = SenderAccount.HashedId;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.SentByUserExternalId = senderUser.ExternalId;
                e.SentByUserId = senderUser.Id;
                e.SentByUserName = senderUser.FullName;
                e.TransferConnectionInvitationId = Id;
            });
        }

        protected TransferConnectionInvitation()
        {
        }

        public void Approve(Account.Account approverAccount, User approverUser)
        {
            RequiresApproverAccountIsTheReceiverAccount(approverAccount);
            RequiresTransferConnectionInvitationIsPending();

            var now = DateTime.UtcNow;
            
            Status = TransferConnectionInvitationStatus.Approved;

            Changes.Add(new TransferConnectionInvitationChange
            {
                Status = Status,
                User = approverUser,
                CreatedDate = now
            });

            Publish<ApprovedTransferConnectionInvitationEvent>(e =>
            {
                e.ApprovedByUserExternalId = approverUser.ExternalId;
                e.ApprovedByUserId = approverUser.Id;
                e.ApprovedByUserName = approverUser.FullName;
                e.CreatedAt = now;
                e.ReceiverAccountHashedId = ReceiverAccount.HashedId;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.SenderAccountHashedId = SenderAccount.HashedId;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.TransferConnectionInvitationId = Id;
            });
        }

        public void Delete(Account.Account deleterAccount, User deleterUser)
        {
            RequiresTransferConnectionInvitationIsRejected();
            RequiresDeleterIsEitherSenderOrReceiver(deleterAccount);

            var now = DateTime.UtcNow;
        
            bool? deletedBySender = null;
            bool? deletedByReceiver = null;

            if (ReceiverAccountId == deleterAccount.Id)
            {
                RequiresDeleterAccountIsTheReceiverAccount(deleterAccount);
                RequiresNotAlreadyDeletedByReceiver();
                DeletedByReceiver = true;
                deletedByReceiver = true;
            }
            else 
            {
                RequiresDeleterAccountIsTheSenderAccount(deleterAccount);
                RequiresNotAlreadyDeletedBySender();
                DeletedBySender = true;
                deletedBySender = true;
            }

            Changes.Add(new TransferConnectionInvitationChange
            {
                DeletedBySender =  deletedBySender,
                DeletedByReceiver = deletedByReceiver,
                User = deleterUser,
                CreatedDate = now
            });

            Publish<DeletedTransferConnectionInvitationEvent>(e =>
            {
                e.CreatedAt = now;
                e.DeletedByAccountId = deleterAccount.Id;
                e.DeletedByUserExternalId = deleterUser.ExternalId;
                e.DeletedByUserId = deleterUser.Id;
                e.DeletedByUserName = deleterUser.FullName;
                e.ReceiverAccountHashedId = ReceiverAccount.HashedId;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.SenderAccountHashedId = SenderAccount.HashedId;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.TransferConnectionInvitationId = Id;
            });
        }

        public void Reject(Account.Account rejectorAccount, User rejectorUser)
        {
            RequiresRejectorAccountIsTheReceiverAccount(rejectorAccount);
            RequiresTransferConnectionInvitationIsPending();

            var now = DateTime.UtcNow;

            Status = TransferConnectionInvitationStatus.Rejected;
            
            Changes.Add(new TransferConnectionInvitationChange
            {
                Status = Status,
                User = rejectorUser,
                CreatedDate = now
            });

            Publish<RejectedTransferConnectionInvitationEvent>(e =>
            {
                e.CreatedAt = now;
                e.ReceiverAccountHashedId = ReceiverAccount.HashedId;
                e.ReceiverAccountId = ReceiverAccount.Id;
                e.ReceiverAccountName = ReceiverAccount.Name;
                e.RejectorUserExternalId = rejectorUser.ExternalId;
                e.RejectorUserId = rejectorUser.Id;
                e.RejectorUserName = rejectorUser.FullName;
                e.SenderAccountHashedId = SenderAccount.HashedId;
                e.SenderAccountId = SenderAccount.Id;
                e.SenderAccountName = SenderAccount.Name;
                e.TransferConnectionInvitationId = Id;
            });
        }

        private void RequiresApproverAccountIsTheReceiverAccount(Account.Account approverAccount)
        {
            if (approverAccount.Id != ReceiverAccount.Id)
                throw new Exception("Requires approver account is the receiver account");
        }

        private void RequiresDeleterAccountIsTheSenderAccount(Account.Account deleterAccount)
        {
            if (deleterAccount.Id != SenderAccount.Id)
                throw new Exception("Requires deleter account is the sender account");
        }

        private void RequiresDeleterAccountIsTheReceiverAccount(Account.Account deleterAccount)
        {
            if (deleterAccount.Id != ReceiverAccount.Id)
                throw new Exception("Requires deleter account is the receiver account");
        }

        private void RequiresDeleterIsEitherSenderOrReceiver(Account.Account deleterAccount)
        {
            if (deleterAccount.Id != ReceiverAccountId && deleterAccount.Id != SenderAccountId)
            {
                throw new Exception("Requires deleter account is either the sender or the receiver");
            }
        }

        private void RequiresNotAlreadyDeletedBySender()
        {
            if (DeletedBySender)
                throw new Exception("Requires not already deleted by sender");
        }

        private void RequiresNotAlreadyDeletedByReceiver()
        {
            if (DeletedByReceiver)
                throw new Exception("Requires not already deleted by receiver");
        }

        private void RequiresRejectorAccountIsTheReceiverAccount(Account.Account rejectorAccount)
        {
            if (rejectorAccount.Id != ReceiverAccount.Id)
                throw new Exception("Requires rejector account is the receiver account");
        }

        private void RequiresTransferConnectionInvitationIsPending()
        {
            if (Status != TransferConnectionInvitationStatus.Pending)
                throw new Exception("Requires transfer connection invitation is pending");
        }

        private void RequiresTransferConnectionInvitationIsRejected()
        {
            if (Status != TransferConnectionInvitationStatus.Rejected)
                throw new Exception("Requires transfer connection invitation is rejected");
        }
    }
}