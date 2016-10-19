using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserQuery : IAsyncRequest<GetUserResponse>
    {
        public long UserId { get; set; }
    }
}
