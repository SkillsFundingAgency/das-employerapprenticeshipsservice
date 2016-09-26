using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.PauseApprenticeship
{
    public sealed class PauseApprenticeshipCommand : IAsyncRequest
    {
        public long EmployerAccountId { get; set; }
        public long CommitmentId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
