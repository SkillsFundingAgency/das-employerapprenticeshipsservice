using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovedByEmployerEventHandler : EventHandler<CohortApprovedByEmployer>
    {
        private readonly IEventHandler<CohortApprovedByEmployer> _handler;

        public CohortApprovedByEmployerEventHandler(
            IEventHandler<CohortApprovedByEmployer> handler,
            IMessageContextInitialisation messageContextInitialisation)
                : base(messageContextInitialisation)
        {
            _handler = handler;
        }

        protected override Task Handle(CohortApprovedByEmployer cohortApprovedByEmployer, CancellationToken cancellationToken = default)
        {
            return _handler.Handle(cohortApprovedByEmployer, cancellationToken);
        }
    }
}
