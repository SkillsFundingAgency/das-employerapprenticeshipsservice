using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovalRequestedByProviderEventHandler : EventHandler<CohortApprovalRequestedByProvider>
    {
        public CohortApprovalRequestedByProviderEventHandler(
            IMessageContextInitialisation messageContextInitialisation,
            IEventHandler<CohortApprovalRequestedByProvider> reservationCreatedEventHandler,
            ILogger<CohortApprovalRequestedByProviderEventHandler> logger)
            : base(messageContextInitialisation, reservationCreatedEventHandler, logger)
        {
        }
    }
}
