using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQuery : IAsyncRequest<GetAccountPayeSchemesResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}