using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
    {
        public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            if (SchemeWasAddedViaAornRoute(message))
            {
                return;
            }

            await context.SendLocal(
                new ImportAccountLevyDeclarationsCommand
                {
                    AccountId = message.AccountId,
                    PayeRef = message.PayeRef,
                });
        }

        private static bool SchemeWasAddedViaAornRoute(AddedPayeSchemeEvent message)
        {
            return !string.IsNullOrEmpty(message.Aorn);
        }
    }
}
