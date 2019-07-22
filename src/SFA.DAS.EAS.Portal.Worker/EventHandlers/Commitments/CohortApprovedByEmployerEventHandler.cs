using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovedByEmployerEventHandler : EventHandler<CohortApprovedByEmployer>
    {
        public CohortApprovedByEmployerEventHandler(
            IMessageContextInitialisation messageContextInitialisation,
            IEventHandler<CohortApprovedByEmployer> reservationCreatedEventHandler,
            ILogger<CohortApprovedByEmployerEventHandler> logger)
            : base(messageContextInitialisation, reservationCreatedEventHandler, logger)
        {
        }
    }
}
