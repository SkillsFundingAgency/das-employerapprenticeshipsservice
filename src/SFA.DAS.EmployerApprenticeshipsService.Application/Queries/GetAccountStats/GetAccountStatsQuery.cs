using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountStats
{
    public class GetAccountStatsQuery : IAsyncRequest<GetAccountStatsResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}
