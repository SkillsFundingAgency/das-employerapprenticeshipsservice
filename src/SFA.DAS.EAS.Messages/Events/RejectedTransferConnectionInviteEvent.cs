using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public class RejectedTransferConnectionInviteEvent : IEvent
    {
        public DateTime CreatedAt { get; set; }
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public Guid RejectorUserExternalId { get; set; }
        public long RejectorUserId { get; set; }
        public string RejectorUserName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public int TransferConnectionInvitationId { get; set; }
    }
}
