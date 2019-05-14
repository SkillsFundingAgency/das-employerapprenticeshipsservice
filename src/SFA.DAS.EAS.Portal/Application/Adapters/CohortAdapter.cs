using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;

namespace SFA.DAS.EAS.Portal.Application.Adapters
{
    public class CohortAdapter : IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>
    {
        public CohortApprovalRequestedCommand Convert(CohortApprovalRequestedByProvider input, IMessageHandlerContext context)
        {
            return new CohortApprovalRequestedCommand(context.MessageId, input.AccountId, input.ProviderId, input.CommitmentId);
        }
    }
}
