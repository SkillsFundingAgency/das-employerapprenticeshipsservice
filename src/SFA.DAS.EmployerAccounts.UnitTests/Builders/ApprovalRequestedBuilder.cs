using SFA.DAS.Commitments.Events;

namespace SFA.DAS.EmployerAccounts.UnitTests.Builders
{
    public class ApprovalRequestedBuilder
    {
        public CohortApprovalRequestedByProvider Build()
        {
            return new CohortApprovalRequestedByProvider();
        }

        public static implicit operator CohortApprovalRequestedByProvider(ApprovalRequestedBuilder instance)
        {
            return instance.Build();
        }
    }
}
