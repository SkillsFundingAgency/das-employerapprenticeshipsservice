using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTask
{
    public class GetTaskQueryRequest : IAsyncRequest<GetTaskQueryResponse>
    {
        public long AccountId { get; set; }
        public long TaskId { get; set; }
    }
}