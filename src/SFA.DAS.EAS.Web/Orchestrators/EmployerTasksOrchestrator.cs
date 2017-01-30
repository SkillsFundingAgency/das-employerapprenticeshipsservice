using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Queries.GetTask;
using SFA.DAS.EAS.Application.Queries.GetTasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.EAS.Web.Orchestrators
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
            var accountId = _hashingService.DecodeValue(accountHashId);

            var response = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                AccountId = accountId
            });

            return new TaskListViewModel
            {
                AccountHashId = accountHashId,
                Tasks = response.Tasks?.Select(x => MapFrom(x)).ToList() ?? new List<TaskListItemViewModel>(0)
            };
        }

        public async Task<TaskViewModel> GetTask(string hashedAccountId, string hashedTaskId, string userId)
        {
            var response = await _mediator.SendAsync(new GetTaskQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                TaskId = _hashingService.DecodeValue(hashedTaskId)
            });

            var taskTemplate = JsonConvert.DeserializeObject<SubmitCommitmentTemplate>(response.Task.Body);

            return new TaskViewModel
            {
                Name = response.Task.Name,
                CreatedOn = response.Task.CreatedOn,
                LinkId = _hashingService.HashValue(taskTemplate.CommitmentId),
                Message = taskTemplate.Message
            };
        }

        private TaskListItemViewModel MapFrom(Tasks.Api.Types.Task task)
        {
            return new TaskListItemViewModel
            {
                HashedTaskId = _hashingService.HashValue(task.Id),
                Name = task.Name,
                Status = task.TaskStatus,
                CreatedOn = task.CreatedOn
            };
        }
    }
}
