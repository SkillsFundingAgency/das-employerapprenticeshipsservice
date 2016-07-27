using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember
{
    public class GetMemberRequest : IAsyncRequest<GetMemberResponse>
    {
        public long AccountId { get; set; }
        public string Email { get; set; }
    }
}