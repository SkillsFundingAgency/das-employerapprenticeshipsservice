using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTasks
{
    public class GetAccountTasksQuery : IAsyncRequest<GetAccountTasksResponse>
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
