﻿using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EAS.Application.Queries.GetTasks
{
    public class GetTasksQueryHandler : IAsyncRequestHandler<GetTasksQueryRequest, GetTasksQueryResponse>
    {
        private readonly ITasksApi _tasksApi;

        public GetTasksQueryHandler(ITasksApi tasksApi)
        {
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));

            _tasksApi = tasksApi;
        }

        public async Task<GetTasksQueryResponse> Handle(GetTasksQueryRequest message)
        {
            var assigneePrfix = message.AssigneeEmployer ? "EMPLOYER" : "PROVIDER";
            var assignee = $"{assigneePrfix}-{message.AccountId}";

            var tasks = await _tasksApi.GetTasks(assignee);

            return new GetTasksQueryResponse
            {
                Tasks = tasks
            };
        }
    }
}