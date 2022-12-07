using System;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class DeletedTransferConnectionRequestEvent
    {
        public long DeletedByAccountId { get; set; }
        public long DeletedByUserId { get; set; }
        public string DeletedByUserName { get; set; }
        public Guid DeletedByUserRef { get; set; }
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public int TransferConnectionRequestId { get; set; }
        public DateTime Created { get; set; }
    }
}
