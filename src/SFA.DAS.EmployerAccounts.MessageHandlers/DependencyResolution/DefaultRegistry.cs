using SFA.DAS.Commitments.Events;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.Commands.Cohort;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<ICommandHandler<CohortApprovalRequestedCommand>>().Use<CohortApprovalRequestedCommandHandler>();
            For<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>>().Use<CohortAdapter>();
        }
    }
}