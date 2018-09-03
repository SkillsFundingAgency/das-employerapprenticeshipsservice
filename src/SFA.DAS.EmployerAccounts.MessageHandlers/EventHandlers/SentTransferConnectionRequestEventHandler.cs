using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class SentTransferConnectionRequestEventHandler : IHandleMessages<SentTransferConnectionRequestEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public SentTransferConnectionRequestEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(SentTransferConnectionRequestEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new SentTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionRequestId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                SentByUserId = message.SentByUserId,
                SentByUserExternalId = message.SentByUserRef,
                SentByUserName = message.SentByUserName,
                CreatedAt = message.Created
            });
        }
    }
}