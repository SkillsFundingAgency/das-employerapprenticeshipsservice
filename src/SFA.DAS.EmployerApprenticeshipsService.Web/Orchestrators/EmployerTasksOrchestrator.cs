using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTask;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerTasksOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerTasksOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }

        public async Task<TaskListViewModel> GetTasks(long accountId, string userId)
        {
            var response = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                AccountId = accountId
            });

            return new TaskListViewModel
            {
                AccountId = accountId,
                Tasks = response.Tasks
            };
        }

        public async Task<TaskViewModel> GetTask(long accountId, long taskId, string userId)
        {
            var response = await _mediator.SendAsync(new GetTaskQueryRequest
            {
                AccountId = accountId,
                TaskId = taskId
            });

            var taskTemplate = JsonConvert.DeserializeObject<SubmitCommitmentTemplate>(response.Task.Body);

            return new TaskViewModel
            {
                AccountId = accountId,
                Task = response.Task,
                LinkId = taskTemplate.CommitmentId,
                Message = taskTemplate.Message
            };
        }
    }
}
