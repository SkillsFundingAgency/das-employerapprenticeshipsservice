using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
    }
}
