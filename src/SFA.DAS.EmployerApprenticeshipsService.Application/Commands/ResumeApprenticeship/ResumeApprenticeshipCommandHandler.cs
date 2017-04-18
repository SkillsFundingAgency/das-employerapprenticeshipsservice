using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

using Task = System.Threading.Tasks.Task;

namespace SFA.DAS.EAS.Application.Commands.ResumeApprenticeship
{
    // TODO: LWA - Are we going to need this class??
    public sealed class ResumeApprenticeshipCommandHandler : AsyncRequestHandler<ResumeApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;
        private readonly ResumeApprenticeshipCommandValidator _validator;

        public ResumeApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
            _validator = new ResumeApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(ResumeApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeship = await _commitmentApi.GetEmployerApprenticeship(message.EmployerAccountId, message.ApprenticeshipId);

            var apprenticeshipSubmission = new ApprenticeshipSubmission { PaymentStatus = PaymentStatus.Active, UserId = message.UserId };
            await _commitmentApi.PatchEmployerApprenticeship(message.EmployerAccountId, message.ApprenticeshipId, apprenticeshipSubmission);
        }
    }
}
