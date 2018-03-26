using System;

namespace SFA.DAS.EAS.Domain.Models.TransferRequests
{
    public class TransferRequest
    {
        public virtual long CommitmentId { get; protected set; }
        public virtual string CommitmentHashedId { get; protected set; }
        public virtual DateTime CreatedDate { get; protected set; }
        public virtual DateTime? ModifiedDate { get; protected set; }
        public virtual Data.Entities.Account.Account ReceiverAccount { get; protected set; }
        public virtual long ReceiverAccountId { get; protected set; }
        public virtual Data.Entities.Account.Account SenderAccount { get; protected set; }
        public virtual long SenderAccountId { get; protected set; }
        public virtual TransferRequestStatus Status { get; protected set; }
        public virtual decimal TransferCost { get; protected set; }

        public TransferRequest(long commitmentId, string commitmentHashedId, Data.Entities.Account.Account receiverAccount, Data.Entities.Account.Account senderAccount, decimal transferCost)
        {
            CommitmentId = commitmentId;
            CommitmentHashedId = commitmentHashedId;
            ReceiverAccount = receiverAccount;
            SenderAccount = senderAccount;
            Status = TransferRequestStatus.Pending;
            TransferCost = transferCost;
            CreatedDate = DateTime.UtcNow;
        }

        protected TransferRequest()
        {
        }

        public void Approved(Data.Entities.Account.Account senderAccount, Data.Entities.Account.Account receiverAccount)
        {
            Status = TransferRequestStatus.Approved;
            ModifiedDate = DateTime.UtcNow;
        }

        public void Rejected(Data.Entities.Account.Account senderAccount, Data.Entities.Account.Account receiverAccount)
        {
            Status = TransferRequestStatus.Rejected;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}