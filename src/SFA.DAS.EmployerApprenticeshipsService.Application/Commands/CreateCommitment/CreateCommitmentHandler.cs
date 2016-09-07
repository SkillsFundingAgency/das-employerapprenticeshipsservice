using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentHandler : AsyncRequestHandler<CreateCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;

        public CreateCommitmentHandler(ICommitmentsApi commitmentApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));

            _commitmentApi = commitmentApi;
        }

        protected override async Task HandleCore(CreateCommitmentCommand message)
        {
            await _commitmentApi.CreateEmployerCommitment(message.commitment.EmployerAccountId, message.commitment);
        }
    }
}
