using SFA.DAS.EmployerAccounts.Commands.CreateCohort;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.Builders
{
    public class CreateCohortCommandBuilder
    {
        public CreateCohortCommand Build()
        {
            return new CreateCohortCommand();
        }
        
        public static implicit operator CreateCohortCommand(CreateCohortCommandBuilder instance)
        {
            return instance.Build();
        }
    }
}
