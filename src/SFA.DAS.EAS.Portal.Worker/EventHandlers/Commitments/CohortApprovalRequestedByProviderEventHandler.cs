using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovalRequestedByProviderEventHandler : EventHandler<CohortApprovalRequestedByProvider>
    {
        private readonly IEventHandler<CohortApprovalRequestedByProvider> _handler;

        public CohortApprovalRequestedByProviderEventHandler(
            IEventHandler<CohortApprovalRequestedByProvider> handler,
            IMessageContextInitialisation messageContextInitialisation)
                : base(messageContextInitialisation)
        {
            _handler = handler;
        }

        protected override Task Handle(CohortApprovalRequestedByProvider cohortApprovalRequestedByProvider, CancellationToken cancellationToken = default)
        {
            return _handler.Handle(cohortApprovalRequestedByProvider, cancellationToken);
        }
    }
}
