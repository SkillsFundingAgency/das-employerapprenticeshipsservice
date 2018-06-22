using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public class DeletedTransferConnectionInviteEvent : IEvent
    {
        public DateTime CreatedAt { get; set; }
        public long DeletedByAccountId { get; set; }
        public Guid DeletedByUserExternalId { get; set; }
        public long DeletedByUserId { get; set; }
        public string DeletedByUserName { get; set; }
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public int TransferConnectionInvitationId { get; set; }
    }
}
