using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTask;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerTasksOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        public EmployerTasksOrchestrator(IMediator mediator, IHashingService hashingService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _mediator = mediator;
            _hashingService = hashingService;
        }

        public async Task<TaskListViewModel> GetTasks(string accountHashId, string userId)
        {
            var response = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                AccountHashId = accountHashId
            });

            return new TaskListViewModel
            {
                AccountHashId = accountHashId,
                Tasks = response.Tasks
            };
        }

        public async Task<TaskViewModel> GetTask(string hashedAccountId, long taskId, string userId)
        {
            var response = await _mediator.SendAsync(new GetTaskQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                TaskId = taskId
            });

            var taskTemplate = JsonConvert.DeserializeObject<SubmitCommitmentTemplate>(response.Task.Body);

            return new TaskViewModel
            {
                HashedAccountId = hashedAccountId,
                Task = response.Task,
                LinkId = _hashingService.HashValue(taskTemplate.CommitmentId),
                Message = taskTemplate.Message
            };
        }
    }
}
