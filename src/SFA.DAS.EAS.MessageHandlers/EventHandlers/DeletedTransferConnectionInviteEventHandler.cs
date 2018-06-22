using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class DeletedTransferConnectionInviteEventHandler : IHandleMessages<DeletedTransferConnectionInviteEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public DeletedTransferConnectionInviteEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(DeletedTransferConnectionInviteEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new DeletedTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionInvitationId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                DeletedByUserId = message.DeletedByUserId,
                DeletedByUserExternalId = message.DeletedByUserExternalId,
                DeletedByUserName = message.DeletedByUserName,
                DeletedByAccountId = message.DeletedByAccountId,
                CreatedAt = message.CreatedAt
            });
        }
    }
}
