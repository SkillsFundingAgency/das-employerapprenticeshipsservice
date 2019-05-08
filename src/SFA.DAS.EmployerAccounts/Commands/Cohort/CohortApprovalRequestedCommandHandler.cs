using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Commands.Cohort
{
    public class CohortApprovalRequestedCommandHandler : ICommandHandler<CohortApprovalRequestedCommand>
    {
        public Task Handle(CohortApprovalRequestedCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: 
            // 1. Get commitment data from CommitmentsApi
            // 2. Get existing account document from store
            // 3. Update document with new details
            //      Check cohort does not already exist in document (ignore?)
            // 4. Persist document back to store

            return Task.CompletedTask;
        }
    }
}