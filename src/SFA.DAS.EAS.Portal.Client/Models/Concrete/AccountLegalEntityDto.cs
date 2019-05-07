using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Models.Concrete
{
    public class AccountLegalEntityDto : IAccountLegalEntityDto<IReservedFundingDto>
    {
        [JsonProperty("accountLegalEntityId")]
        public long AccountLegalEntityId { get; private set; }

        [JsonProperty("legalEntityName")]
        public string LegalEntityName { get; private set; }
        
        [JsonProperty("reservedFundings")]
        public IEnumerable<IReservedFundingDto> ReservedFundings { get; private set; }
    }
}