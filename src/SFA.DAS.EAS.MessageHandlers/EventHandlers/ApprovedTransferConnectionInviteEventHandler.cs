using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class ApprovedTransferConnectionInviteEventHandler : IHandleMessages<ApprovedTransferConnectionInviteEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public ApprovedTransferConnectionInviteEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        public async Task Handle(ApprovedTransferConnectionInviteEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new ApprovedTransferConnectionInvitationEvent
            {
                TransferConnectionInvitationId = message.TransferConnectionInvitationId,
                SenderAccountId = message.SenderAccountId,
                SenderAccountHashedId = message.SenderAccountHashedId,
                SenderAccountName = message.SenderAccountName,
                ReceiverAccountId = message.ReceiverAccountId,
                ReceiverAccountHashedId = message.ReceiverAccountHashedId,
                ReceiverAccountName = message.ReceiverAccountName,
                ApprovedByUserId = message.ApprovedByUserId,
                ApprovedByUserExternalId = message.ApprovedByUserExternalId,
                ApprovedByUserName = message.ApprovedByUserName,
                CreatedAt = message.CreatedAt
            });
        }
    }
}
