using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleQuery : IAsyncRequest<GetUserAccountRoleResponse>
    {
        public string ExternalUserId { get; set; }
        public long AccountId { get; set; }
    }
}
