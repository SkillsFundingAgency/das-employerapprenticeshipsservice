using MediatR;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommand : IAsyncRequest<CreateCommitmentCommandResponse>
    {
        public Commitment Commitment { get; set; }
        public string Message { get; set; }
    }
}
