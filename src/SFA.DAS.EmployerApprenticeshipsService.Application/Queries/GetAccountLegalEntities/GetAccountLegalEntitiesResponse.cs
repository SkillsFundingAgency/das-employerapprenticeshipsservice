using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesResponse
    {
        public List<AccountSpecificLegalEntity> Entites { get; set; }
    }
}