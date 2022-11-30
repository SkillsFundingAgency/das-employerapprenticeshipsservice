using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Events.Messages;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class ApprovedTransferConnectionRequestEventHandler : IHandleMessages<ApprovedTransferConnectionRequestEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public ApprovedTransferConnectionRequestEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(ApprovedTransferConnectionRequestEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new ApprovedTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionRequestId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                ApprovedByUserId = message.ApprovedByUserId,
                ApprovedByUserExternalId = message.ApprovedByUserRef,
                ApprovedByUserName = message.ApprovedByUserName,
                CreatedAt = message.Created
            });
        }
    }
}
