using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTask
{
    public class GetTaskQueryRequest : IAsyncRequest<GetTaskQueryResponse>
    {
        public long AccountId { get; set; }
        public long TaskId { get; set; }

        public bool AssigneeEmployer { get; set; }
    }
}