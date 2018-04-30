using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleQuery : IAsyncRequest<GetUserAccountRoleResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}
