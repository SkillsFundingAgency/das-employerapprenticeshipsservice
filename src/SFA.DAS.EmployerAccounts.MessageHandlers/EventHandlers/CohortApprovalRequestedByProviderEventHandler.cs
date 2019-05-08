using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Commitments.Events;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.Commands.Cohort;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class CohortApprovalRequestedByProviderEventHandler : IHandleMessages<CohortApprovalRequestedByProvider>
    {
        private readonly ICommandHandler<CohortApprovalRequestedCommand> _handler;
        private readonly IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand> _adapter;

        public CohortApprovalRequestedByProviderEventHandler(
            ICommandHandler<CohortApprovalRequestedCommand> handler,
            IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand> adapter)
        {
            _handler = handler;
            _adapter = adapter;
        }

        public Task Handle(CohortApprovalRequestedByProvider message, IMessageHandlerContext context)
        {
            return _handler.Handle(_adapter.Convert(message));
        }
    }
}
