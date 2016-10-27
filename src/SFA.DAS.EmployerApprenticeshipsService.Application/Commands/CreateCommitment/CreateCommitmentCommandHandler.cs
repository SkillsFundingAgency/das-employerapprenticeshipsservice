using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandHandler : AsyncRequestHandler<CreateCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;

        public CreateCommitmentCommandHandler(ICommitmentsApi commitmentApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));

            _commitmentApi = commitmentApi;
        }

        protected override async Task HandleCore(CreateCommitmentCommand message)
        {
            try
            {
                await _commitmentApi.CreateEmployerCommitment(message.Commitment.EmployerAccountId, message.Commitment);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
