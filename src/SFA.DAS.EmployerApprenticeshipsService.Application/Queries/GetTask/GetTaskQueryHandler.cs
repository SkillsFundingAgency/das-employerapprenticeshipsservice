using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EAS.Application.Queries.GetTask
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
            var assigneePrfix = message.AssigneeEmployer ? "EMPLOYER" : "PROVIDER";
            var assignee = $"{assigneePrfix}-{message.AccountId}";

            var task = await _tasksApi.GetTask(assignee, message.TaskId);

            return new GetTaskQueryResponse
            {
                Task = task
            };
        }
    }
}