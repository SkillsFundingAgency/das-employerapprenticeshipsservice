using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovalRequestedByProviderEventHandler : EventHandler<CohortApprovalRequestedByProvider>
    {
        //todo: rename command to directive, not event
        public CohortApprovalRequestedByProviderEventHandler(IPortalCommand<CohortApprovalRequestedByProvider> cohortApprovalRequestedCommand, IMessageContext messageContext)
            : base(cohortApprovalRequestedCommand, messageContext)
        {
        }
    }
}
