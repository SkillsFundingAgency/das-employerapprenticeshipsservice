using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleQuery : IAsyncRequest<GetUserAccountRoleResponse>
    {
        public string HashedAccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
