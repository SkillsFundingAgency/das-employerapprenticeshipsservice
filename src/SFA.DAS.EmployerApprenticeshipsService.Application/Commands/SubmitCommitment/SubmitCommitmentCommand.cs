using MediatR;

using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EAS.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommand : IAsyncRequest
    {
        public long EmployerAccountId { get; set; }
        public long CommitmentId { get; set; }

        public string HashedCommitmentId { get; set; }

        public string Message { get; set; }

        public bool CreateTask { get; set; }

        public LastAction LastAction { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }

        public string UserId { get; set; }
    }
}
