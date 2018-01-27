using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("transfer_connection_invitation_sent")]
    public class TransferConnectionInvitationSentMessage
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public long TransferConnectionId { get; protected set; }
        public long SenderAccountId { get; protected set; }
        public string SenderAccountName { get; protected set; }
        public long ReceiverAccountId { get; protected set; }
        public string ReceiverAccountName { get; protected set; }
        public string CreatorName { get; protected set; }
        public string CreatorUserRef { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public TransferConnectionInvitationSentMessage()
        {
        }

        public TransferConnectionInvitationSentMessage(
            long transferConnectionId,
            long senderAccountId,
            string senderAccountName,
            long receiverAccountId,
            string receiverAccountName,
            string creatorName,
            string creatorUserRef)
        {
            TransferConnectionId = transferConnectionId;
            SenderAccountId = senderAccountId;
            SenderAccountName = senderAccountName;
            ReceiverAccountId = receiverAccountId;
            ReceiverAccountName = receiverAccountName;
            CreatorName = creatorName;
            CreatorUserRef = creatorUserRef;
            CreatedAt = DateTime.UtcNow;
        }
    }
}