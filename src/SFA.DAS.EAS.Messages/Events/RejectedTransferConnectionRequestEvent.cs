using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EAS.Messages.Events
{
    public class RejectedTransferConnectionRequestEvent : Event
    {
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public Guid RejectorUserExternalId { get; set; }
        public long RejectorUserId { get; set; }
        public string RejectorUserName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public int TransferConnectionRequestId { get; set; }
    }
}
