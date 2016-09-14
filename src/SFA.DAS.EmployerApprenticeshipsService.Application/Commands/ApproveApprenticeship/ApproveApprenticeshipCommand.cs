using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship
{
    public sealed class ApproveApprenticeshipCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public long CommitmentId { get; set; }
        public long EmployerAccountId { get; set; }
    }
}
