using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUser
{
    public class GetUserQuery : IAsyncRequest<GetUserResponse>
    {
        public long UserId { get; set; }
    }
}
