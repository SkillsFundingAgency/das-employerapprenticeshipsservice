using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks
{
    public class GetTasksQueryRequest : IAsyncRequest<GetTasksQueryResponse>
    {
        public string AccountHashId { get; set; }
    }
}