using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks
{
    public class GetTasksQueryRequest : IAsyncRequest<GetTasksQueryResponse>
    {
        public long AccountId { get; set; }
    }
}