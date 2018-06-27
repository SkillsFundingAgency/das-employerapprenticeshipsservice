using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class RejectedTransferConnectionInviteEventHandler : IHandleMessages<RejectedTransferConnectionInviteEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public RejectedTransferConnectionInviteEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(RejectedTransferConnectionInviteEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new RejectedTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionInvitationId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                RejectorUserId = message.RejectorUserId,
                RejectorUserExternalId = message.RejectorUserExternalId,
                RejectorUserName = message.RejectorUserName,
                CreatedAt = message.CreatedAt
            });
        }
    }
}
