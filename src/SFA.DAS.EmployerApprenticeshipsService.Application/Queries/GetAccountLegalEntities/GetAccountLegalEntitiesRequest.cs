﻿using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
    {
        public string HashedId { get; set; }
        public string UserId { get; set; }
        public bool  SignedOnly { get; set; }
    }
}