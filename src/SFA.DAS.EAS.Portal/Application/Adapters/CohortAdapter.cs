using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;

namespace SFA.DAS.EAS.Portal.Application.Adapters
{
    public class CohortAdapter : IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>
    {
        public CohortApprovalRequestedCommand Convert(CohortApprovalRequestedByProvider input)
        {
            return new CohortApprovalRequestedCommand(input.AccountId, input.ProviderId, input.CommitmentId);
        }
    }
}
