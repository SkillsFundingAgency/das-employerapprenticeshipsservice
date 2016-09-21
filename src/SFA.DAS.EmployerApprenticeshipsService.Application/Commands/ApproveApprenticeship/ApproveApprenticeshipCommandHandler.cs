using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship
{
    public sealed class ApproveApprenticeshipCommandHandler : AsyncRequestHandler<ApproveApprenticeshipCommand>
    {
        private ICommitmentsApi _commitmentsApi;
        private readonly ITasksApi _tasksApi;
        private readonly ApproveApprenticeshipCommandValidator _validator;

        public ApproveApprenticeshipCommandHandler(ICommitmentsApi commitmentsApi, ITasksApi tasksApi)
        {
            _commitmentsApi = commitmentsApi;
            _validator = new ApproveApprenticeshipCommandValidator();
            _tasksApi = tasksApi;
        }

        protected override async Task HandleCore(ApproveApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            // TODO: LWA - Validated Employer is that of the commitment and apprenticeship is in the commitment.
            var commitment = await _commitmentsApi.GetEmployerCommitment(message.EmployerAccountId, message.CommitmentId);

            await _commitmentsApi.PatchApprenticeship(message.EmployerAccountId, message.CommitmentId, message.ApprenticeshipId, Commitments.Api.Types.ApprenticeshipStatus.Approved);

            await CreateTask(commitment);
        }

        private async Task CreateTask(Commitment commitment)
        {
            var task = TaskFactory.Create(commitment.ProviderId.Value, "This is the body of the task.");

            await _tasksApi.CreateTask(task.Assignee, task);
        }
    }
}
