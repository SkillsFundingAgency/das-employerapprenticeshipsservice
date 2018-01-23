using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("transfer_connection_invitation_sent")]
    public class TransferConnectionInvitationSentMessage : AccountMessageBase
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public long TransferConnectionId { get; protected set; }
        public long SenderAccountId { get; protected set; }

        public TransferConnectionInvitationSentMessage()
        {
        }

        public TransferConnectionInvitationSentMessage(long transferConnectionId, long senderAccountId, long receiverAccountId, string creatorName, string creatorUserRef)
            : base(receiverAccountId, creatorName, creatorUserRef)
        {
            TransferConnectionId = transferConnectionId;
            SenderAccountId = senderAccountId;
        }
    }
}