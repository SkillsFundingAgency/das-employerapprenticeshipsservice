using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Commands.CreateCohort
{
    public class CreateCohortCommandHandler : ICommandHandler<CreateCohortCommand>
    {
        public Task Handle(CreateCohortCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}