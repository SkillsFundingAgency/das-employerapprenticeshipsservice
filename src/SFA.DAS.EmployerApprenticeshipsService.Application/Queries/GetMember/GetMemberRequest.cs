using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetMember
{
    public class GetMemberRequest : IAsyncRequest<GetMemberResponse>
    {
        public string HashedAccountId { get; set; }
        public string Email { get; set; }
    }
}