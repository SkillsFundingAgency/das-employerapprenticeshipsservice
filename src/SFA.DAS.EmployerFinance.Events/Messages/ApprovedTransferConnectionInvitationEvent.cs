using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerFinance.Events.Messages
{
    [Serializable]
    [MessageGroup("approved_transfer_connection_invitation")]
    public class ApprovedTransferConnectionInvitationEvent
    {
        public Guid ApprovedByUserExternalId { get; set; }
        public long ApprovedByUserId { get; set; }
        public string ApprovedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public int TransferConnectionInvitationId { get; set; }
    }
}