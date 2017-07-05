using MediatR;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.EAS.Application.Commands.ResolveRequestedChanges
{
    public class ResolveRequestedChangesCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }

        public TriageStatus TriageStatus { get; set; }

        public bool Approved { get; set; }

        public string UserId { get; set; }

        public long AccountId { get; set; }
    }
}