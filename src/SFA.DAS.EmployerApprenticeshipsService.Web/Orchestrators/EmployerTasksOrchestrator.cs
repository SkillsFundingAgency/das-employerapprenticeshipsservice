using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

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
    }
}
