using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Api.Types.Templates;
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

            var taskTemplate = new CreateCommitmentTemplate
            {
                CommitmentId = message.CommitmentId,
                Message = message.Message,
                Source = $"EMPLOYER-{message.EmployerAccountId}"
            };

            var task = new Tasks.Api.Types.Task
            {
                Assignee = $"PROVIDER-{commitment.ProviderId}",
                TaskTemplateId = CreateCommitmentTemplate.TemplateId,
                Name = "Create Commitment",
                Body = JsonConvert.SerializeObject(taskTemplate)
            };

            await _tasksApi.CreateTask(task.Assignee, task);
        }
    }
}
