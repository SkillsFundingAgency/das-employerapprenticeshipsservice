using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
    {
        public string HashedLegalEntityId { get; set; }
        public Guid ExternalUserId { get; set; }
        public bool  SignedOnly { get; set; }
    }
}