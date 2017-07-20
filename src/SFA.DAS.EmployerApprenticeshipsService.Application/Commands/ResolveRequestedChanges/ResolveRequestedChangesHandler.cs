using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.EAS.Application.Commands.ResolveRequestedChanges
{
    public class ResolveRequestedChangesHandler : AsyncRequestHandler<ResolveRequestedChangesCommand>
    {
        private readonly IDataLockApi _dataLockApi;

        public ResolveRequestedChangesHandler(IDataLockApi dataLockApi)
        {
            _dataLockApi = dataLockApi;
        }

        protected override async Task HandleCore(ResolveRequestedChangesCommand command)
        {
            var dataLockUpdateType = command.Approved 
                ? DataLockUpdateType.ApproveChanges 
                : DataLockUpdateType.RejectChanges;

            await _dataLockApi.PatchDataLocks(
                command.ApprenticeshipId,
                new DataLocksTriageResolutionSubmission
                    {
                        DataLockUpdateType = dataLockUpdateType,
                        TriageStatus = command.TriageStatus,
                        UserId = command.UserId
                    });
        }
    }
}