using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SubmitCommitment
{
    using SFA.DAS.Commitments.Api.Types;

    public sealed class SubmitCommitmentCommand : IAsyncRequest
    {
        public long EmployerAccountId { get; set; }
        public long CommitmentId { get; set; }
        public string Message { get; set; }

        public bool CreateTask { get; set; }

        public AgreementStatus AgreementStatus { get; set; }
    }
}
