using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTeamMembers
{
    public class GetTeamMembersRequest : IAsyncRequest<GetTeamMembersResponse>
    {
        public string HashedAccountId { get; set; }
    }
}
