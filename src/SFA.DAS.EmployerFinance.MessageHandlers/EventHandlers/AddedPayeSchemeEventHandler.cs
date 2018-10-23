using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public AddedPayeSchemeEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(
                new ImportAccountLevyDeclarationsCommand
                {
                    AccountId = message.AccountId,
                    PayeRef = message.PayeRef,
                });
        }
    }
}
