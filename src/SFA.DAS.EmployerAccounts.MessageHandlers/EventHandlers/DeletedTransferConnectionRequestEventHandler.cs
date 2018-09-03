using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class DeletedTransferConnectionRequestEventHandler : IHandleMessages<DeletedTransferConnectionRequestEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public DeletedTransferConnectionRequestEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(DeletedTransferConnectionRequestEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new DeletedTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionRequestId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                DeletedByUserId = message.DeletedByUserId,
                DeletedByUserExternalId = message.DeletedByUserRef,
                DeletedByUserName = message.DeletedByUserName,
                DeletedByAccountId = message.DeletedByAccountId,
                CreatedAt = message.Created
            });
        }
    }
}