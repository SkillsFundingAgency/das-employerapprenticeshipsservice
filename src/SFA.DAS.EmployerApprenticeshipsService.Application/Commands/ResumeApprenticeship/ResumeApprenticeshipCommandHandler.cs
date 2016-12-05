using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using Task = System.Threading.Tasks.Task;

namespace SFA.DAS.EAS.Application.Commands.ResumeApprenticeship
{
    public sealed class ResumeApprenticeshipCommandHandler : AsyncRequestHandler<ResumeApprenticeshipCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly ResumeApprenticeshipCommandValidator _validator;

        public ResumeApprenticeshipCommandHandler(ICommitmentsApi commitmentApi)
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

            await _commitmentApi.PatchEmployerApprenticeship(message.EmployerAccountId, message.CommitmentId, message.ApprenticeshipId, PaymentStatus.Active);
        }
    }
}
