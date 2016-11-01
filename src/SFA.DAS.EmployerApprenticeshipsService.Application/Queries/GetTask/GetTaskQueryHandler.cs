using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EAS.Application.Queries.GetTask
{
    public class GetTaskQueryHandler : IAsyncRequestHandler<GetTaskQueryRequest, GetTaskQueryResponse>
    {
        private readonly IHashingService _hashingService;
        private readonly ITasksApi _tasksApi;

        public GetTaskQueryHandler(ITasksApi tasksApi, IHashingService hashingService)
        {
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _tasksApi = tasksApi;
            _hashingService = hashingService;
        }

        public async Task<GetTaskQueryResponse> Handle(GetTaskQueryRequest message)
        {
            var assignee = $"EMPLOYER-{message.AccountId}";

            var task = await _tasksApi.GetTask(assignee, message.TaskId);

            return new GetTaskQueryResponse
            {
                Task = task
            };
        }
    }
}