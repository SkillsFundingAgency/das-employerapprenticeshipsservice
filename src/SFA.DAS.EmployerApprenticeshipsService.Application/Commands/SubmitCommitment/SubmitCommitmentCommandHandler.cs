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
        private readonly SubmitCommitmentCommandValidator _validator;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
            _validator = new SubmitCommitmentCommandValidator();
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _commitmentApi.PatchEmployerCommitment(message.EmployerAccountId, message.CommitmentId, CommitmentStatus.Active);
        }
    }
}
