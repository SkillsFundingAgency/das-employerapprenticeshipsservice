using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserRequest : IAsyncRequest<GetUserResponse>
    {
        public long UserId { get; set; }
    }
}
