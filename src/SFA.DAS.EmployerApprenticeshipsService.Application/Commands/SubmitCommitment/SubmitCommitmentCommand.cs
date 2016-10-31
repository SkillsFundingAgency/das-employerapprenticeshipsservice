using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommand : IAsyncRequest
    {
        public long EmployerAccountId { get; set; }
        public long CommitmentId { get; set; }
        public string Message { get; set; }
        public string SaveOrSend { get; set; }
    }
}
