using System;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
    {
        public long Id { get; set; }
        public string UserId { get; set; }
    }
}