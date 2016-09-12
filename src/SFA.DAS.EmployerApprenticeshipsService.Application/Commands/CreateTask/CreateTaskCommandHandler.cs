using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tasks.Api.Client;
using ApiTypes = SFA.DAS.Tasks.Domain.Entities;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateTask
{
    public sealed class CreateTaskCommandHandler : AsyncRequestHandler<CreateTaskCommand>
    {
        private readonly ITasksApi _tasksApi;

        public CreateTaskCommandHandler(ITasksApi tasksApi)
        {
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));

            _tasksApi = tasksApi;
        }
        protected override async Task HandleCore(CreateTaskCommand message)
        {
            var assignee = $"PROVIDER-{message.ProviderId}";

            var task = new ApiTypes.Task
            {
                Assignee = assignee,
                Body = "A new commitment is ready for review."
            };

            await _tasksApi.CreateTask(task.Assignee, task);
        }
    }
}
