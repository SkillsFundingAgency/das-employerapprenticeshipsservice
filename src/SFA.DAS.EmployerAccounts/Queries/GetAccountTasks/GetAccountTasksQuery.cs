using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTasks
{
    public class GetAccountTasksQuery : IAsyncRequest<GetAccountTasksResponse>
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
