using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
    {
        public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            await context.SendLocal(new CreateAccountPayeCommand(message.AccountId, message.PayeRef, message.SchemeName, message.Aorn));

            if (SchemeWasAddedViaAornRoute(message))
            {
                return;
            }

            await context.SendLocal(new ImportAccountLevyDeclarationsCommand(message.AccountId, message.PayeRef));
        }

        private static bool SchemeWasAddedViaAornRoute(AddedPayeSchemeEvent message)
        {
            return !string.IsNullOrEmpty(message.Aorn);
        }
    }
}
