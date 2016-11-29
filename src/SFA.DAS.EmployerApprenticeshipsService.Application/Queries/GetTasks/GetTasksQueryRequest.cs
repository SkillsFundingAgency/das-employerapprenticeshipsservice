using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTasks
{
    public class GetTasksQueryRequest : IAsyncRequest<GetTasksQueryResponse>
    {
        public long AccountId { get; set; }
    }
}