using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            await _commitmentApi.PatchEmployerCommitment(message.EmployerAccountId, message.CommitmentId, CommitmentStatus.Active);
        }
    }
}
