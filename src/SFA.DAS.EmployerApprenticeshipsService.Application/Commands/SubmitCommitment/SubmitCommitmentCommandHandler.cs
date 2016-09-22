using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Tasks.Api.Client;
using Task = System.Threading.Tasks.Task;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly ITasksApi _tasksApi;
        private readonly SubmitCommitmentCommandValidator _validator;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentApi, ITasksApi tasksApi)
        {
            _commitmentApi = commitmentApi;
            _validator = new SubmitCommitmentCommandValidator();
            _tasksApi = tasksApi;
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            // TODO: LWA - Validate commitment is for the correct employer
            var commitment = await _commitmentApi.GetEmployerCommitment(message.EmployerAccountId, message.CommitmentId);

            await _commitmentApi.PatchEmployerCommitment(message.EmployerAccountId, message.CommitmentId, CommitmentStatus.Active);

            await CreateTask(commitment);
        }

        private async Task CreateTask(Commitment commitment)
        {
            var task = TaskFactory.Create(commitment.ProviderId.Value, "SubmitCommitment", "This is the body of the task.");

            await _tasksApi.CreateTask(task.Assignee, task);
        }
    }
}
