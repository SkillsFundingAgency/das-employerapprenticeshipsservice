using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.CreateAccountPaye;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
    {
        public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            var createSchemeTask = context.SendLocal(new CreateAccountPayeCommand(message.AccountId, message.PayeRef,
                message.SchemeName, message.Aorn));

            var importTask = context.SendLocal(
                new ImportAccountLevyDeclarationsCommand
                {
                    AccountId = message.AccountId,
                    PayeRef = message.PayeRef,
                });

            await Task.WhenAll(createSchemeTask, importTask);
        }
    }
}
