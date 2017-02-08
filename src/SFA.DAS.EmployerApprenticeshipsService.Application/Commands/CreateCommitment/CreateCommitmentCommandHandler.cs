using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.EAS.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandHandler :
        IAsyncRequestHandler<CreateCommitmentCommand, CreateCommitmentCommandResponse>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly ITasksApi _tasksApi;

        public CreateCommitmentCommandHandler(ICommitmentsApi commitmentApi, ITasksApi tasksApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));

            _commitmentApi = commitmentApi;
            _tasksApi = tasksApi;
        }

        public async Task<CreateCommitmentCommandResponse> Handle(CreateCommitmentCommand request)
        {
            // TODO: This needs to return just the Id
            var commitment = await _commitmentApi.CreateEmployerCommitment(request.Commitment.EmployerAccountId, request.Commitment);

            if (request.Commitment.CommitmentStatus == CommitmentStatus.Active)
            {
                await CreateTask(request, commitment.Id);
            }

            return new CreateCommitmentCommandResponse
            {
                CommitmentId = commitment.Id
            };
        }
        private async Task CreateTask(CreateCommitmentCommand request, long commitmentId)
        {
            var taskTemplate = new CreateCommitmentTemplate
            {
                CommitmentId = commitmentId,
                Message = request.Message,
                Source = $"EMPLOYER-{request.Commitment.EmployerAccountId}"
            };

            var task = new Tasks.Api.Types.Task
            {
                Assignee = $"PROVIDER-{request.Commitment.ProviderId}",
                TaskTemplateId = CreateCommitmentTemplate.TemplateId,
                Name = "Create Commitment",
                Body = JsonConvert.SerializeObject(taskTemplate)
            };

            await this._tasksApi.CreateTask(task.Assignee, task);
        }
    }
}
