using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember
{
    public class GetMemberRequest : IAsyncRequest<GetMemberResponse>
    {
        public string HashedId { get; set; }
        public string Email { get; set; }
    }
}