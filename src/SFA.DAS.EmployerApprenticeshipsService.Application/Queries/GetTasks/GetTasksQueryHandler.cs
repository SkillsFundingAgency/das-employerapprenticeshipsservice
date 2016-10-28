using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EAS.Application.Queries.GetTasks
{
    public class GetTasksQueryHandler : IAsyncRequestHandler<GetTasksQueryRequest, GetTasksQueryResponse>
    {
        private readonly IHashingService _hashingService;
        private readonly ITasksApi _tasksApi;

        public GetTasksQueryHandler(ITasksApi tasksApi, IHashingService hashingService)
        {
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _tasksApi = tasksApi;
            _hashingService = hashingService;
        }

        public async Task<GetTasksQueryResponse> Handle(GetTasksQueryRequest message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashId);
            var assignee = $"EMPLOYER-{accountId}";

            var tasks = await _tasksApi.GetTasks(assignee);

            return new GetTasksQueryResponse
            {
                Tasks = tasks
            };
        }
    }
}