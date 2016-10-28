using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTasks
{
    public class GetTasksQueryRequest : IAsyncRequest<GetTasksQueryResponse>
    {
        public string AccountHashId { get; set; }
    }
}