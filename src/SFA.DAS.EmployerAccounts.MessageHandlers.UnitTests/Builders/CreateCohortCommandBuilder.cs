using SFA.DAS.EmployerAccounts.Commands.Cohort;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.Builders
{
    public class CreateCohortCommandBuilder
    {
        public CohortApprovalRequestedCommand Build()
        {
            return new CohortApprovalRequestedCommand(123);
        }
        
        public static implicit operator CohortApprovalRequestedCommand(CreateCohortCommandBuilder instance)
        {
            return instance.Build();
        }
    }
}
