using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Events.Messages;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class RejectedTransferConnectionRequestEventHandler : IHandleMessages<RejectedTransferConnectionRequestEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public RejectedTransferConnectionRequestEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(RejectedTransferConnectionRequestEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new RejectedTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionRequestId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                RejectorUserId = message.RejectorUserId,
                RejectorUserExternalId = message.RejectorUserRef,
                RejectorUserName = message.RejectorUserName,
                CreatedAt = message.Created
            });
        }
    }
}