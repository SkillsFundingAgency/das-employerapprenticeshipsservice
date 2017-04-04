using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTasks
{
    public class GetTasksQueryRequest : IAsyncRequest<GetTasksQueryResponse>
    {
        public long Id { get; set; }

        public bool IsAssigneeEmployer { get; set; }
    }
}