using SFA.DAS.EmployerAccounts.Events.Cohort;

namespace SFA.DAS.EmployerAccounts.UnitTests.Builders
{
    public class CohortCreatedBuilder
    {
        public CohortCreated Build()
        {
            return new CohortCreated();
        }

        public static implicit operator CohortCreated(CohortCreatedBuilder instance)
        {
            return instance.Build();
        }
    }
}
