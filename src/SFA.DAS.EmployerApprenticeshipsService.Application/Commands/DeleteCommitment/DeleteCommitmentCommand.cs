using MediatR;

namespace SFA.DAS.EAS.Application.Commands.DeleteCommitment
{
    public class DeleteCommitmentCommand : IAsyncRequest
    {
        public long AccountId { get; set; }

        public long CommitmentId { get; set; }

        public string UserId { get; set; }
    }
}