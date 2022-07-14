using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerFinance.Events.Messages
{
    [Serializable]
    [MessageGroup("sent_transfer_connection_invitation")]
    public class SentTransferConnectionInvitationEvent
    {
        public DateTime CreatedAt { get; set; }
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public Guid SentByUserExternalId { get; set; }
        public long SentByUserId { get; set; }
        public string SentByUserName { get; set; }
        public int TransferConnectionInvitationId { get; set; }
    }
}