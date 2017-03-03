using MediatR;

namespace SFA.DAS.EAS.Application.Commands.ResumeApprenticeship
{
    public sealed class ResumeApprenticeshipCommand : IAsyncRequest
    {
        public long EmployerAccountId { get; set; }
        public long CommitmentId { get; set; }
        public long ApprenticeshipId { get; set; }

        public string UserId { get; set; }
    }
}
