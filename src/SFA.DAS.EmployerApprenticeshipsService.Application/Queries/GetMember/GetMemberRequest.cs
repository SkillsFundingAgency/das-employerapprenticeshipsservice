using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetMember
{
    public class GetMemberRequest : IAsyncRequest<GetMemberResponse>
    {
        public string HashedId { get; set; }
        public string Email { get; set; }
    }
}