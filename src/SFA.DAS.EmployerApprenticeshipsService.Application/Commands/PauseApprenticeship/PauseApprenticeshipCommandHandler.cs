using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using Task = System.Threading.Tasks.Task;

namespace SFA.DAS.EAS.Application.Commands.PauseApprenticeship
{
    public sealed class PauseApprenticeshipCommandHandler : AsyncRequestHandler<PauseApprenticeshipCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly PauseApprenticeshipCommandValidator _validator;

        public PauseApprenticeshipCommandHandler(ICommitmentsApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
            _validator = new PauseApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(PauseApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeship = await _commitmentApi.GetEmployerApprenticeship(message.EmployerAccountId, message.CommitmentId, message.ApprenticeshipId);

            await _commitmentApi.PatchEmployerApprenticeship(message.EmployerAccountId, message.CommitmentId, message.ApprenticeshipId, PaymentStatus.Paused);
        }
    }
}
