using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class SentTransferConnectionInviteEventHandler : IHandleMessages<SentTransferConnectionInviteEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public SentTransferConnectionInviteEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        public async Task Handle(SentTransferConnectionInviteEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new SentTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionInvitationId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                SentByUserId = message.SentByUserId,
                SentByUserExternalId = message.SentByUserExternalId,
                SentByUserName = message.SentByUserName,
                CreatedAt = message.CreatedAt
            });
        }
    }
}
