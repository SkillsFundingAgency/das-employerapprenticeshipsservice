using SFA.DAS.Commitments.Events;
using SFA.DAS.EmployerAccounts.Commands.Cohort;

namespace SFA.DAS.EmployerAccounts.Adapters
{
    public class CohortAdapter : IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>
    {
        public CohortApprovalRequestedCommand Convert(CohortApprovalRequestedByProvider input)
        {
            return new CohortApprovalRequestedCommand(input.AccountId);
        }
    }
}
