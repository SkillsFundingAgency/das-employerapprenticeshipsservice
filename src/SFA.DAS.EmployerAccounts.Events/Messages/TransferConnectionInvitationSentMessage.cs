using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("transfer_connection_invitation_sent")]
    public class TransferConnectionInvitationSentMessage
    {
        [Obsolete("Please use 'TransferConnectionInvitationId' instead.")]
        public long TransferConnectionId { get; set; }

        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }

        [Obsolete("Please use 'SenderUserName' instead.")]
        public string CreatorName { get; set; }

        [Obsolete("Please use 'SenderUserExternalId' instead.")]
        public string CreatorUserRef { get; set; }

        public DateTime CreatedAt { get; set; }
        public long TransferConnectionInvitationId { get; set; }
        public Guid SenderUserExternalId { get; set; }
        public string SenderUserName { get; set; }
    }
}