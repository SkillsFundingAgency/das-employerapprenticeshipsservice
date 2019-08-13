using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedLegalEntityEventHandler : IHandleMessages<AddedLegalEntityEvent>
    {
        public Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
        {
            return context.SendLocal(new CreateAccountLegalEntityCommand(message.AccountLegalEntityId, null, null, null, null,
                message.AccountId, message.LegalEntityId));
        }
    }
}
