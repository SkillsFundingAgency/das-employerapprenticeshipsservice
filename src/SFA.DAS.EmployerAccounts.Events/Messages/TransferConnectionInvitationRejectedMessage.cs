using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("transfer_connection_invitation_rejected")]
    public class TransferConnectionInvitationRejectedMessage
    {
        public long TransferConnectionInvitationId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public Guid RejectorUserExternalId { get; set; }
        public string RejectorUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}