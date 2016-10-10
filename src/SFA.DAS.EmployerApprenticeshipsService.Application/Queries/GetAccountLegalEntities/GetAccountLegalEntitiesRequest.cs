using System;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
    {
        public string HashedId { get; set; }
        public string UserId { get; set; }
    }
}