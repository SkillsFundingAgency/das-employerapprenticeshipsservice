using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

using Task = System.Threading.Tasks.Task;

namespace SFA.DAS.EAS.Application.Commands.PauseApprenticeship
{
    // TODO: LWA - Are we going to need this class
    public sealed class PauseApprenticeshipCommandHandler : AsyncRequestHandler<PauseApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;
        private readonly PauseApprenticeshipCommandValidator _validator;

        public PauseApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
            _validator = new PauseApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(PauseApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeship = await _commitmentApi.GetEmployerApprenticeship(message.EmployerAccountId, message.ApprenticeshipId);

            var apprenticeshipSubmission = new ApprenticeshipSubmission { PaymentStatus = PaymentStatus.Paused, UserId = message.UserId };
            await _commitmentApi.PatchEmployerApprenticeship(message.EmployerAccountId, message.ApprenticeshipId, apprenticeshipSubmission);
        }
    }
}
