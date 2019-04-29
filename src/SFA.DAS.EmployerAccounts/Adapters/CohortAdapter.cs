using SFA.DAS.EmployerAccounts.Commands.CreateCohort;
using SFA.DAS.EmployerAccounts.Events.Cohort;

namespace SFA.DAS.EmployerAccounts.Adapters
{
    public class CohortAdapter : IAdapter<CohortCreated, CreateCohortCommand>
    {
        public CreateCohortCommand Convert(CohortCreated input)
        {
            return new CreateCohortCommand();
        }
    }
}
