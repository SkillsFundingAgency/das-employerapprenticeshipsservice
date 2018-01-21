using System;

namespace SFA.DAS.EAS.Domain.Models.TransferConnection
{
    public class TransferConnection
    {
        public long Id { get; set; }
        public long SenderUserId { get; set; }
        public long SenderAccountId { get; set; }
        public long ReceiverAccountId { get; set; }
        public TransferConnectionStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}