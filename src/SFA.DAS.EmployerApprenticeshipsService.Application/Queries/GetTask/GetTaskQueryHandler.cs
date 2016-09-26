using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTask
{
    public class GetTaskQueryHandler : IAsyncRequestHandler<GetTaskQueryRequest, GetTaskQueryResponse>
    {
        private readonly ITasksApi _tasksApi;

        public GetTaskQueryHandler(ITasksApi tasksApi)
        {
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));
            _tasksApi = tasksApi;
        }

        public async Task<GetTaskQueryResponse> Handle(GetTaskQueryRequest message)
        {
            var assignee = $"EMPLOYER-{message.AccountId}";

            var task = await _tasksApi.GetTask(message.TaskId, assignee);

            return new GetTaskQueryResponse
            {
                Task = task
            };
        }
    }
}