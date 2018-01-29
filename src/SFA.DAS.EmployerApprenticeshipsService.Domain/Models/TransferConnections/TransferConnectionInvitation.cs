using System;

namespace SFA.DAS.EAS.Domain.Models.TransferConnections
{
    public class TransferConnectionInvitation
    {
        public long Id { get; set; }
        public long CreatorUserId { get; set; }
        public long? SenderUserId { get; set; }
        public long SenderAccountId { get; set; }
        public long ReceiverAccountId { get; set; }
        public TransferConnectionInvitationStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}