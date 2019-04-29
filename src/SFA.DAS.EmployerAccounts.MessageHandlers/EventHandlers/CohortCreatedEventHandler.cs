using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.Commands.CreateCohort;
using SFA.DAS.EmployerAccounts.Events.Cohort;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class CohortCreatedEventHandler : IHandleMessages<CohortCreated>
    {
        private readonly ICommandHandler<CreateCohortCommand> _handler;
        private readonly IAdapter<CohortCreated, CreateCohortCommand> _adapter;

        public CohortCreatedEventHandler(
            ICommandHandler<CreateCohortCommand> handler,
            IAdapter<CohortCreated, CreateCohortCommand> adapter)
        {
            _handler = handler;
            _adapter = adapter;
        }

        public Task Handle(CohortCreated message, IMessageHandlerContext context)
        {
            return _handler.Handle(_adapter.Convert(message));
        }
    }
}
