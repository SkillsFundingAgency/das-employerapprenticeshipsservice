using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
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

            var apprenticeshipSubmission = new ApprenticeshipSubmission
            {
                PaymentStatus = PaymentStatus.Paused,
                UserId = message.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName }
            };
            await _commitmentApi.PatchEmployerApprenticeship(message.EmployerAccountId, message.ApprenticeshipId, apprenticeshipSubmission);
        }
    }
}
